namespace WikipediaExtractor
{
    /// <summary>
    /// Wikipedia index file page index item.
    /// </summary>
    public class PageIndexItem
    {
        public string PageTitle { get; set; }

        public long ByteStart { get; set; }

        public int PageId { get; set; }

        public static char Separator { get; set; } = ':';

        public PageIndexItem Parse(string line)
        {
            // Split index line by separator.
            var parts = line.Split(new [] { Separator }, 3);
            
            // Initialize PageIndexItem.
            return new PageIndexItem
            {
                ByteStart = long.Parse(parts[0]),
                PageId = int.Parse(parts[1]),
                PageTitle = parts[2]
            };
        }

        public override string ToString()
        {
            return ByteStart + Separator.ToString() + PageId + Separator + PageTitle;
        }
    }
}