using Microsoft.VisualStudio.TestTools.UnitTesting;
using WikipediaExtractor;

namespace UnitTests
{
    [TestClass]
    public class PageIndexItemTests
    {
        [TestMethod]
        public void CanParsePageIndexItem()
        {
            var expected = new PageIndexItem
            {
                ByteStart = 24083825,
                PageId = 5291,
                PageTitle = "Namespace:Page Title"
            };

            var actual = new PageIndexItem().Parse(expected.ToString());

            Assert.AreEqual(expected.ByteStart, actual.ByteStart);
            Assert.AreEqual(expected.PageId, actual.PageId);
            Assert.AreEqual(expected.PageTitle, actual.PageTitle);
        }
    }
}