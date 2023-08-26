using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WikipediaExtractor.Tests.UnitTests
{
    [TestClass]
    public class PageIndexSearcherTests
    {
        [TestMethod]
        public void CanSearchPageIndexById()
        {
            // Arrange.
            var pageIndexItems = CreatePageIndex(100);
            var pageIndexStream = CreatePageIndexMemoryStream(pageIndexItems);
            var pageIdQuery = new List<int> { 1, 4, 9, 10, 12, 19 }; // Search for these page IDs.

            // Act.
            IEnumerable<PageIndexItem> pageIndexSearchResults = null;
            using (var indexSearcher = new PageIndexSearcher(pageIndexStream))
                pageIndexSearchResults = indexSearcher.Search(pageIdQuery);

            // Assert.
            Assert.AreEqual(pageIdQuery.Count, pageIndexSearchResults.Count());
            AssertPageIndexSearchResults(pageIndexItems, pageIndexSearchResults);
        }

        [TestMethod]
        public void CanSearchPageIndexByTitle()
        {
            // Arrange.
            var pageIndexItems = CreatePageIndex(100);
            var pageIndexStream = CreatePageIndexMemoryStream(pageIndexItems);
            var pageIdQuery = new List<int> { 1, 4, 9, 10, 12, 19 };         
            var pageTitleQuery = pageIdQuery.Select(x => "Page " + x).ToList(); // Add "Page " title to ID string query.  

            // Act.
            IEnumerable<PageIndexItem> pageIndexSearchResults = null;
            using (var indexSearcher = new PageIndexSearcher(pageIndexStream))
                pageIndexSearchResults = indexSearcher.Search(pageTitleQuery);

            // Assert.
            Assert.AreEqual(pageTitleQuery.Count, pageIndexSearchResults.Count());
            AssertPageIndexSearchResults(pageIndexItems, pageIndexSearchResults);
        }

        [TestMethod]
        public void CanSearchPageIndexByRegex()
        {
            // Arrange.
            var pageIndexItems = CreatePageIndex(100);
            var pageIndexStream = CreatePageIndexMemoryStream(pageIndexItems);
            var pageRegexQuery = new Regex(@"^(Page 1)"); // Search for any titles which contain text "Page 1"

            // Act.
            IEnumerable<PageIndexItem> pageIndexSearchResults = null;
            using (var indexSearcher = new PageIndexSearcher(pageIndexStream))
                pageIndexSearchResults = indexSearcher.Search(pageRegexQuery);

            // Assert.
            Assert.AreEqual(11, pageIndexSearchResults.Count());
            AssertPageIndexSearchResults(pageIndexItems, pageIndexSearchResults);
        }

        private IEnumerable<PageIndexItem> CreatePageIndex(int totalPages)
        {
            // Create page index items.
            var pageIndexItems = new List<PageIndexItem>();
            for (var pageNumber = 0; pageNumber < totalPages; pageNumber++)
            {
                // Add page index item to collection.
                pageIndexItems.Add(new PageIndexItem
                {
                    PageId = pageNumber,
                    PageTitle = "Page " + pageNumber
                });
            }
            return pageIndexItems;
        }

        private MemoryStream CreatePageIndexMemoryStream(IEnumerable<PageIndexItem> pageIndexItems)
        {
            // Create stream and writer.    
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            
            // Write PageIndexItem collection to stream.
            foreach (var item in pageIndexItems)
                sw.WriteLine(item.ToString());
            
            // Reset stream position.
            sw.Flush();
            ms.Position = 0;
            
            return ms;
        }

        private void AssertPageIndexSearchResults(IEnumerable<PageIndexItem> pageIndexItems, 
            IEnumerable<PageIndexItem> pageIndexSearchResults)
        {
            foreach (var pageIndexSearchResult in pageIndexSearchResults)
            {
                // Get actual page index item by search result page ID.
                var expectedIndex = pageIndexItems.FirstOrDefault(x => x.PageId == pageIndexSearchResult.PageId);
                
                // Assert.
                Assert.IsNotNull(expectedIndex);
                Assert.AreEqual(expectedIndex.ToString(), pageIndexSearchResult.ToString());
            }
        }
    }
}