using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.BZip2;

namespace WikipediaExtractor
{
    /// <summary>
    /// Extracts XML page data from a multistream Wikipedia data dump using byte offset positions read from index items.
    /// </summary>
    public class DataDumpReader : IDisposable, IDataDumpReader
    {
        private readonly Stream DataDumpStream;       
        
        public event EventHandler<DataDumpPageFoundEventArgs> DataDumpPageFound;

        public DataDumpReader(string path)
        {
            DataDumpStream = new FileStream(path, FileMode.Open);
        }

        public DataDumpReader(Stream dataDumpStream)
        {
            DataDumpStream = dataDumpStream ?? throw new ArgumentNullException("dataDumpStream");
        }

        public IEnumerable<XElement> Search(IEnumerable<PageIndexItem> pageIndexItems)
        {
            var pageIndex = pageIndexItems.GroupBy(x => x.ByteStart).ToDictionary(x => x.Key, x => x.ToList());
            var results = new List<XElement>();
            foreach (var pageOffset in pageIndex.Keys)
            {
                var streamBytes = ReadStream(DataDumpStream, pageOffset);
                var streamText = "<xml>" + System.Text.Encoding.Default.GetString(streamBytes) + "</xml>";
                var xmlDoc = XDocument.Parse(streamText);
                foreach (var pageIndexItem in pageIndex[pageOffset])
                {
                    var pageElement = xmlDoc.Element("xml")
                        .Elements("page")
                        .Where(x=> x.Element("id").Value == pageIndexItem.PageId.ToString()).FirstOrDefault();
                    results.Add(pageElement);
                    OnDataDumpPageFound(results.Count, pageIndexItems.Count(), pageElement, pageElement.Element("title").Value);
                }

            }
            return results;
        }

        private byte[] ReadStream(Stream dataDumpStream, long offset)
        {
            dataDumpStream.Seek(offset, SeekOrigin.Begin);
            using (var decompressedStream = new MemoryStream())
            {
                BZip2.Decompress(DataDumpStream, decompressedStream, false);
                return decompressedStream.ToArray();
            }
        }

        private void OnDataDumpPageFound(int numberOfHits, int queryLength, XElement page, string pageTitle)
        {
            DataDumpPageFound?.Invoke(this, new DataDumpPageFoundEventArgs
            {
                ProgressPercentage = (int)((double)numberOfHits / queryLength * 100.0),
                Page = page,
                PageTitle = pageTitle
            });
        }

        public void Dispose()
        {
            DataDumpStream?.Dispose();
        }
    }
}