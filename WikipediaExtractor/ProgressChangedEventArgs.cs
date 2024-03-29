namespace WikipediaExtractor
{
    public class ProgressChangedEventArgs
    {
        public long BytesRead { get; set; }
        public long TotalBytes { get; set; }
        public int ProgressPercentage { get; set; }
    }
}