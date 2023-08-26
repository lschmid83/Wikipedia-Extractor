using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WikipediaExtractor.Tests.UnitTests
{

    [TestClass]
    public class PageIndexItemTests
    {
        [TestMethod]
        public void CanParsePageIndexItem()
        {
            // Arrange.
            var expectedPageIndexItem = new PageIndexItem
            {
                ByteStart = 24083825,
                PageId = 5291,
                PageTitle = "Namespace:Page Title"
            };

            // Act.
            var actualPageIndexItem = new PageIndexItem().Parse(expectedPageIndexItem.ToString());

            // Assert.
            Assert.AreEqual(expectedPageIndexItem.ByteStart, actualPageIndexItem.ByteStart);
            Assert.AreEqual(expectedPageIndexItem.PageId, actualPageIndexItem.PageId);
            Assert.AreEqual(expectedPageIndexItem.PageTitle, actualPageIndexItem.PageTitle);
        }
    }
}