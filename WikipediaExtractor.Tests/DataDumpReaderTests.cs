using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WikipediaExtractor;

namespace IntegrationTests
{
    [TestClass]
    public class DataDumpReaderTests
    {
        [TestMethod]
        public void CanReadPageFromDataDump()
        {
            var pageIndex = new List<PageIndexItem>();
            using (var dataDumpReader = new DataDumpReader(CreateBZip2MemoryStream(10, 100, out pageIndex)))
            {
                var results = dataDumpReader.Search(pageIndex).ToList();
                Assert.AreEqual(pageIndex.Count, results.Count());
                for (var i = 0; i < pageIndex.Count; i++)
                {
                    Assert.AreEqual(pageIndex[i].PageTitle, results[i].Element("title").Value);
                    Assert.AreEqual(pageIndex[i].PageId, int.Parse(results[i].Element("id").Value));
                }
            }
        }

        private MemoryStream CreateBZip2MemoryStream(int streamCount, int pageCount, out List<PageIndexItem> pageIndexItems)
        {
            pageIndexItems = new List<PageIndexItem>();
            var bzip2Stream = new MemoryStream();
            var pageNumber = 1;
            for (var stream = 0; stream < streamCount; stream++)
            {
                var compressText = string.Empty;
                for (var page = 0; page < pageCount; page++)
                {
                    var pageNode = new XElement("page");
                    pageNode.Add(new XElement("id", pageNumber));
                    pageNode.Add(new XElement("title", "Page" + pageNumber));
                    compressText += pageNode + Environment.NewLine;
                    pageIndexItems.Add(new PageIndexItem()
                    {
                        ByteStart = bzip2Stream.Length,
                        PageId = pageNumber,
                        PageTitle = "Page" + pageNumber
                    });
                    pageNumber++;
                }
                var compressedBytes = BZip2Compress(Encoding.UTF8.GetBytes(compressText));
                bzip2Stream.Write(compressedBytes, 0, compressedBytes.Length);
            }
            return bzip2Stream;
        }

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