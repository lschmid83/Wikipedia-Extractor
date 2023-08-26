using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WikipediaExtractor
{
    public interface IDataDumpReader
    {
        event EventHandler<PageFoundEventArgs> PageFound;

        void Dispose();
        IEnumerable<XElement> Search(IEnumerable<PageIndexItem> pageIndexItems);
    }
}