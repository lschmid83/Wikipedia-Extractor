using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WikipediaExtractor.Tests.UnitTests
{
    [TestClass]
    public class DataDumpReaderTests
    {
        [TestMethod]
        public void CanReadPagesFromDataDump()
        {
            // Arrange.
            var pageIndexItems = new List<PageIndexItem>();
            using (var dataDumpReader = new DataDumpReader(CreateBZip2MemoryStream(10, 100, out pageIndexItems)))
            {
                // Create page index query.                
                var pageIndexQuery = pageIndexItems.Where((x, i) => i % 10 == 0).ToList(); // Select every 10th index item.

                // Act.
                var dataDumpSearchResults = dataDumpReader.Search(pageIndexQuery).ToList();

                // Assert.
                Assert.AreEqual(pageIndexQuery.Count(), dataDumpSearchResults.Count());
                for(var i = 0; i < pageIndexQuery.Count(); i++)
                {
                    Assert.AreEqual(pageIndexQuery[i].PageTitle, dataDumpSearchResults[i].Element("title").Value);
                    Assert.AreEqual(pageIndexQuery[i].PageId, int.Parse(dataDumpSearchResults[i].Element("id").Value));
                }
            }
        }

        /// <summary>
        /// Creates a multistream BZip2 archive memorystream and outputs the page index with bytestart positions.
        /// </summary>
        /// <param name="streamCount">Number of streams in archive.</param>
        /// <param name="streamPageCount">Number of page elements per stream.</param>
        /// <param name="pageIndexItems">Collection of PageIndexItem objects.</param>
        /// <returns></returns>
        private MemoryStream CreateBZip2MemoryStream(int streamCount, int streamPageCount, out List<PageIndexItem> pageIndexItems)
        {
            pageIndexItems = new List<PageIndexItem>();
            var bzip2Stream = new MemoryStream();
            var pageNumber = 0;
            
            // Create separate streams for multistream archive.
            for (var stream = 0; stream < streamCount; stream++)
            {
                // Create page elements in stream.
                var stringToCompress = string.Empty;
                for (var page = 0; page < streamPageCount; page++)
                {
                    // Create page element.
                    var pageElement = new XElement("page");
                    pageElement.Add(new XElement("id", pageNumber));
                    pageElement.Add(new XElement("title", "Page " + pageNumber));
                    
                    // Add page element to string to compress.
                    stringToCompress += pageElement + Environment.NewLine;
                    
                    // Add page to index output.
                    pageIndexItems.Add(new PageIndexItem()
                    {
                        ByteStart = bzip2Stream.Length, // Set bytestart to length of last stream compressed.
                        PageId = pageNumber,
                        PageTitle = "Page " + pageNumber
                    });
                    pageNumber++;
                }

                // Compress current stream containing page items.
                var compressedBytes = BZip2Compress(Encoding.UTF8.GetBytes(stringToCompress));
                bzip2Stream.Write(compressedBytes, 0, compressedBytes.Length);
            }
            return bzip2Stream;
        }

        /// <summary>
        /// Compresses a byte array to a BZip2 stream.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private byte[] BZip2Compress(byte[] bytes)
        {
            using (var source = new MemoryStream(bytes))
            {
                using (var target = new MemoryStream())
                {
                    BZip2.Compress(source, target, true, 1);
                    return target.ToArray();
                }
            }
        }
    }
}