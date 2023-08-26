namespace WikipediaExtractor
{
    /// <summary>
    /// ProgressChanged event arguments.
    /// </summary>
    public class ProgressChangedEventArgs
    {
        public long BytesRead { get; set; }
        public long TotalBytes { get; set; }
        public int ProgressPercentage { get; set; }
    }
}