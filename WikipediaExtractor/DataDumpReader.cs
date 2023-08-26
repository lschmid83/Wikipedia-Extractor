using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.BZip2;

namespace WikipediaExtractor
{
    /// <summary>
    /// Extracts XML page data from a multistream Wikipedia data dump archive using byte offset 
    /// positions read from index file items.
    /// </summary>
    public class DataDumpReader : IDisposable, IDataDumpReader
    {
        // File streams.
        private readonly Stream DataDumpStream;       
        
        // Progress events.
        public event EventHandler<PageFoundEventArgs> PageFound;

        /// <summary>
        /// Constructs DataDumpReader with file path.
        /// </summary>
        public DataDumpReader(string path)
        {
            DataDumpStream = new FileStream(path, FileMode.Open);
        }

        /// <summary>
        /// Constructs DataDumpReader with file stream.
        /// </summary>
        public DataDumpReader(Stream dataDumpStream)
        {
            DataDumpStream = dataDumpStream ?? throw new ArgumentNullException("dataDumpStream");
        }

        /// <summary>
        /// Searches a multistream Wikipedia data dump for page elements using index items positions.
        /// </summary>
        /// <param name="pageIndexItems">Collection of PageIndexItems.</param>
        /// <returns>XElement containing page data.</returns>
        public IEnumerable<XElement> Search(IEnumerable<PageIndexItem> pageIndexItems)
        {
            // Group PageIndexItem objects by bytestart position.
            var groupedPageIndexItems = pageIndexItems.GroupBy(x => x.ByteStart).ToDictionary(x => x.Key, x => x.ToList());
            
            // Loop through page offset byte positions.
            var results = new List<XElement>();
            foreach (var pageOffset in groupedPageIndexItems.Keys)
            {
                // Read stream from BZip2 archive.
                var streamBytes = ReadStream(DataDumpStream, pageOffset);
                
                // Add an <xml> element to page collection stream and convert to string.
                var streamText = "<xml>" + System.Text.Encoding.Default.GetString(streamBytes) + "</xml>";
                
                // Parse the XML page element.
                var xmlDoc = XDocument.Parse(streamText);
                
                // Loop through PageIndexItem objects in offset group.
                foreach (var pageIndexItem in groupedPageIndexItems[pageOffset])
                {
                    // Find page by ID in page collection XML element.
                    var pageElement = xmlDoc.Element("xml")
                        .Elements("page")
                        .Where(x=> x.Element("id").Value == pageIndexItem.PageId.ToString()).FirstOrDefault();

                    // Add to results and report progress.
                    if (pageElement != null)
                    {
                        results.Add(pageElement);
                        OnPageFound(results.Count, pageIndexItems.Count(), pageElement, pageElement.Element("title").Value);
                    }
                }

            }
            return results;
        }

        /// <summary>
        /// Reads a stream from a multistream BZip2 archive at the offset position.
        /// </summary>
        /// <param name="dataDumpStream">BZip2 multistream archive file stream.</param>
        /// <param name="offset">Byte offset position where stream starts.</param>
        /// <returns>Byte array containing BZip2 stream.</returns>
        private byte[] ReadStream(Stream dataDumpStream, long offset)
        {
            dataDumpStream.Seek(offset, SeekOrigin.Begin);
            using (var decompressedStream = new MemoryStream())
            {
                BZip2.Decompress(DataDumpStream, decompressedStream, false);
                return decompressedStream.ToArray();
            }
        }

        /// <summary>
        /// Raises the PageFound event to report progress.
        /// </summary>
        private void OnPageFound(int numberOfHits, int queryLength, XElement page, string pageTitle)
        {
            PageFound?.Invoke(this, new PageFoundEventArgs
            {
                ProgressPercentage = (int)((double)numberOfHits / queryLength * 100.0),
                Page = page,
                PageTitle = pageTitle
            });
        }

        /// <summary>
        /// Releases all resources used by the DataDumpReader.
        /// </summary>
        public void Dispose()
        {
            DataDumpStream?.Dispose();
        }
    }
}