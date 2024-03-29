using System;

namespace WikipediaExtractor
{
    public class PageIndexItemFoundEventArgs : EventArgs
    {
        public PageIndexItem PageIndexItem { get; set; }
    }
}