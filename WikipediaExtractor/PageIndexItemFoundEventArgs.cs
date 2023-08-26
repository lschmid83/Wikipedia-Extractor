using System;

namespace WikipediaExtractor
{
    /// <summary>
    /// PageIndexIndexItemFound event arguments.
    /// </summary>
    public class PageIndexItemFoundEventArgs : EventArgs
    {
        public PageIndexItem PageIndexItem { get; set; }
    }
}