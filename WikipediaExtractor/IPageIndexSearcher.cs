using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WikipediaExtractor
{
    public interface IPageIndexSearcher
    {
        event EventHandler<PageIndexItemFoundEventArgs> PageIndexItemFound;
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        void Dispose();
        IEnumerable<PageIndexItem> Search(List<int> pageIds);
        IEnumerable<PageIndexItem> Search(List<string> pageTitles);
        IEnumerable<PageIndexItem> Search(Regex regex);
        IEnumerable<PageIndexItem> SearchIndex(List<int> pageIds, List<string> pageTitles, Regex regex);
    }
}