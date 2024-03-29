using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WikipediaExtractor
{
    /// <summary>
    /// Searches for page index items by ID, title and regular expression in a Wikipedia data dump index and reports progress.
    /// </summary>
    public class PageIndexSearcher : IDisposable, IPageIndexSearcher
    {
        private readonly Stream PageIndexStream;
        private readonly StreamReader PageIndexStreamReader;

        public event EventHandler<PageIndexItemFoundEventArgs> PageIndexItemFound;
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        public PageIndexSearcher(string path)
        {
            PageIndexStream = new FileStream(path, FileMode.Open);
            PageIndexStreamReader = new StreamReader(PageIndexStream);
        }

        public PageIndexSearcher(Stream pageIndexStream)
        {
            PageIndexStream = pageIndexStream ?? throw new ArgumentNullException("pageIndexStream");
            PageIndexStreamReader = new StreamReader(PageIndexStream);
        }

        public IEnumerable<PageIndexItem> Search(List<int> pageIds)
        {
            return SearchIndex(pageIds, null, null);
        }
        
        public IEnumerable<PageIndexItem> Search(List<string> pageTitles)
        {
            return SearchIndex(null, pageTitles, null);
        }

        public IEnumerable<PageIndexItem> Search(Regex regex)
        {
            return SearchIndex(null, null, regex);
        }

        public IEnumerable<PageIndexItem> SearchIndex(List<int> pageIds, List<string> pageTitles, Regex regex)
        {
            var result = new List<PageIndexItem>();
            var line = string.Empty;
            var i = 0;
            while ((line = PageIndexStreamReader.ReadLine()) != null)
            {
                var pageIndexItem = new PageIndexItem().Parse(line);

                if (pageIds != null && pageIds.Contains(pageIndexItem.PageId))
                    AddResult(pageIndexItem, result);

                if (pageTitles != null && pageTitles.Contains(pageIndexItem.PageTitle))
                    AddResult(pageIndexItem, result);

                if (regex != null && regex.IsMatch(pageIndexItem.PageTitle))
                    AddResult(pageIndexItem, result);

                if (i % 1000 == 0)
                    OnProgressChanged(PageIndexStreamReader.BaseStream.Position, PageIndexStreamReader.BaseStream.Length);
                i++;
            }
            return result;
        }

        private void AddResult(PageIndexItem pageIndexItem, List<PageIndexItem> result)
        {
            result.Add(pageIndexItem);
            OnPageIndexItemFound(pageIndexItem);
        }
        
        private void OnProgressChanged(long bytesRead, long totalBytes)
        {
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs
            {
                BytesRead = bytesRead,
                TotalBytes = totalBytes,
                ProgressPercentage = (int)((double)bytesRead / totalBytes * 100.0)
            });
        }

        private void OnPageIndexItemFound(PageIndexItem pageIndexItem)
        {
            PageIndexItemFound?.Invoke(this, new PageIndexItemFoundEventArgs
            {
                PageIndexItem = pageIndexItem
            });
        }

        public void Dispose()
        {
            PageIndexStream?.Dispose();
            PageIndexStreamReader?.Dispose();
        }
    }
}