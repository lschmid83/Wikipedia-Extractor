using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WikipediaExtractor;

namespace IntegrationTests
{
    [TestClass]
    public class PageIndexSearcherTests
    {
        [TestMethod]
        public void CanSearchPageIndexById()
        {
            var pageIndex = CreatePageIndexes(100);
            var pageIndexStream = CreatePageIndexMemoryStream(pageIndex);
            var pageIdQuery = new List<int> { 1, 4, 9, 10, 12, 19 };

            IEnumerable<PageIndexItem> results = null;
            using (var indexSearcher = new PageIndexSearcher(pageIndexStream))
                results = indexSearcher.Search(pageIdQuery);

            Assert.AreEqual(pageIdQuery.Count, results.Count());
            CheckIndexContainsSearchResults(pageIndex, results);
        }

        [TestMethod]
        public void CanSearchPageIndexByTitle()
        {
            var pageIndex = CreatePageIndexes(100);
            var pageIndexStream = CreatePageIndexMemoryStream(pageIndex);
            var pageIdQuery = new List<int> { 1, 4, 9, 10, 12, 19 };
            var pageTitleQuery = pageIdQuery.Select(x => "Page" + x).ToList();

            IEnumerable<PageIndexItem> results = null;
            using (var indexSearcher = new PageIndexSearcher(pageIndexStream))
                results = indexSearcher.Search(pageTitleQuery);

            Assert.AreEqual(pageTitleQuery.Count, results.Count());
            CheckIndexContainsSearchResults(pageIndex, results);
        }

        [TestMethod]
        public void CanSearchPageIndexByRegex()
        {
            var pageIndex = CreatePageIndexes(100);
            var pageIndexStream = CreatePageIndexMemoryStream(pageIndex);
            var pageRegexQuery = new Regex(@"^(Page1)");

            IEnumerable<PageIndexItem> results = null;
            using (var indexSearcher = new PageIndexSearcher(pageIndexStream))
                results = indexSearcher.Search(pageRegexQuery);

            Assert.AreEqual(11, results.Count());
            CheckIndexContainsSearchResults(pageIndex, results);
        }

        private void CheckIndexContainsSearchResults(IEnumerable<PageIndexItem> pageIndex, IEnumerable<PageIndexItem> searchResult)
        {
            foreach (var actual in searchResult)
            {
                var expected = pageIndex.FirstOrDefault(x => x.PageTitle == actual.PageTitle);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.ToString(), actual.ToString());
            }
        }

        private IEnumerable<PageIndexItem> CreatePageIndexes(int totalPages)
        {
            var pageIndexItems = new List<PageIndexItem>();
            for (var pageNumber = 0; pageNumber < totalPages; pageNumber++)
            {
                pageIndexItems.Add(new PageIndexItem
                {
                    PageId = pageNumber,
                    PageTitle = "Page" + pageNumber
                });
            }
            return pageIndexItems;
        }

        private MemoryStream CreatePageIndexMemoryStream(IEnumerable<PageIndexItem> pageIndexItems)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            foreach (var item in pageIndexItems)
                sw.WriteLine(item.ToString());
            sw.Flush();
            ms.Position = 0;
            return ms;
        }
    }
}