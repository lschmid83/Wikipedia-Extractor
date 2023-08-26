using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WikipediaExtractor
{
    /// <summary>
    /// Searches for page index items by ID, title and regular expression in a Wikipedia data 
    /// dump index file and reports progress.
    /// </summary>
    public class PageIndexSearcher : IDisposable, IPageIndexSearcher
    {
        // File streams.
        private readonly Stream PageIndexStream;
        private readonly StreamReader PageIndexStreamReader;

        // Progress events.
        public event EventHandler<PageIndexItemFoundEventArgs> PageIndexItemFound;
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// Constructs PageIndexSearcher with file path.
        /// </summary>
        public PageIndexSearcher(string path)
        {
            PageIndexStream = new FileStream(path, FileMode.Open);
            PageIndexStreamReader = new StreamReader(PageIndexStream);
        }

        /// <summary>
        /// Constructs PageIndexSearcher with file stream.
        /// </summary>
        public PageIndexSearcher(Stream pageIndexStream)
        {
            PageIndexStream = pageIndexStream ?? throw new ArgumentNullException("pageIndexStream");
            PageIndexStreamReader = new StreamReader(PageIndexStream);
        }

        /// <summary>
        /// Search for page index items by ID.
        /// </summary>
        public IEnumerable<PageIndexItem> Search(List<int> pageIds)
        {
            return SearchIndex(pageIds, null, null);
        }
        
        /// <summary>
        /// Search for page index items by title.
        /// </summary>
        public IEnumerable<PageIndexItem> Search(List<string> pageTitles)
        {
            return SearchIndex(null, pageTitles, null);
        }

        /// <summary>
        /// Search for page index items by title using regular expression.
        /// </summary>
        public IEnumerable<PageIndexItem> Search(Regex regex)
        {
            return SearchIndex(null, null, regex);
        }

        /// <summary>
        /// Searches for page index items by page ID, title or regular expression.
        /// </summary>
        public IEnumerable<PageIndexItem> SearchIndex(List<int> pageIds, List<string> pageTitles, Regex regex)
        {
            var result = new List<PageIndexItem>();
            var i = 0;
            string indexLine;
            
            // Read page index item file lines.
            while ((indexLine = PageIndexStreamReader.ReadLine()) != null)
            {
                // Parse PageIndexItem object from line read from file.
                var pageIndexItem = new PageIndexItem().Parse(indexLine);

                // Page ID match.
                if (pageIds != null && pageIds.Contains(pageIndexItem.PageId))
                    AddResult(pageIndexItem, result);

                // Page title match.
                if (pageTitles != null && pageTitles.Contains(pageIndexItem.PageTitle))
                    AddResult(pageIndexItem, result);

                // Regex title match.
                if (regex != null && regex.IsMatch(pageIndexItem.PageTitle))
                    AddResult(pageIndexItem, result);

                // Raise progress changed event every 1000 items processed.
                if (i % 1000 == 0)
                    OnProgressChanged(PageIndexStreamReader.BaseStream.Position, PageIndexStreamReader.BaseStream.Length);
                i++;
            }
            return result;
        }

        /// <summary>
        /// Adds page index item to result set and raises index item found event.
        /// </summary>
        private void AddResult(PageIndexItem pageIndexItem, List<PageIndexItem> result)
        {
            result.Add(pageIndexItem);
            OnPageIndexItemFound(pageIndexItem);
        }
        
        /// <summary>
        /// Raises the ProgressChanged event to report file read progress.
        /// </summary>
        private void OnProgressChanged(long bytesRead, long totalBytes)
        {
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs
            {
                BytesRead = bytesRead,
                TotalBytes = totalBytes,
                ProgressPercentage = (int)((double)bytesRead / totalBytes * 100.0)
            });
        }

        /// <summary>
        /// Raises the PageIndexItemFound event with index item found.
        /// </summary>
        private void OnPageIndexItemFound(PageIndexItem pageIndexItem)
        {
            PageIndexItemFound?.Invoke(this, new PageIndexItemFoundEventArgs
            {
                PageIndexItem = pageIndexItem
            });
        }

        /// <summary>
        /// Releases all resources used by the PageIndexSearcher.
        /// </summary>
        public void Dispose()
        {
            PageIndexStream?.Dispose();
            PageIndexStreamReader?.Dispose();
        }
    }
}