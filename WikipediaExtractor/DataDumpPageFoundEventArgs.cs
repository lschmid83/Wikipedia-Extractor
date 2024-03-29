using System.Xml.Linq;

namespace WikipediaExtractor
{
    public class DataDumpPageFoundEventArgs
    {
        public XElement Page { get; set; }
        public string PageTitle { get; set; }
        public int ProgressPercentage { get; set; }
    }
}