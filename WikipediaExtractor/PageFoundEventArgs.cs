using System.Xml.Linq;

namespace WikipediaExtractor
{
    /// <summary>
    /// PageFound event arguments.
    /// </summary>
    public class PageFoundEventArgs
    {
        public XElement Page { get; set; }
        public string PageTitle { get; set; }
        public int ProgressPercentage { get; set; }
    }
}