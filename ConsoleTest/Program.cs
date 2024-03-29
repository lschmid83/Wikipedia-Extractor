using WikipediaExtractor;

internal class Program
{
    private static void Main(string[] args)
    {
        var pageTitles = new List<string>
        {
            "Software development",
            "Microsoft Visual Studio",
            "JavaScript"
        };

        using (var indexSearcher = new PageIndexSearcher(@"F:\enwiki-20190701-pages-articles-multistream-index.txt"))
        {
            var pageIndexItems = indexSearcher.Search(pageTitles);
            foreach (PageIndexItem pii in pageIndexItems)
            {
                Console.WriteLine(pii.PageId + ": " + pii.PageTitle);
            }

            using (var dataDumpReader = new DataDumpReader(@"F:\enwiki-20190701-pages-articles-multistream.xml.bz2"))
            {
                var results = dataDumpReader.Search(pageIndexItems);
                foreach (var result in results) 
                { 
                    Console.WriteLine(result.Name + ": " + result.Value);               
                }
            }
        }
    }
}