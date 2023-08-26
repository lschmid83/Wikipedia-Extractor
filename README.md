# Wikipedia Extractor

Wikipedia Extractor is a lightweight C# library which can be used to extract XML page data from a Wikipedia data dump. It makes use of the index file included with the compressed data dump to find the position of the page and quickly retrieve it from the archive. It was developed using Visual Studio 2022.

The current URL for the data dumps is https://dumps.wikimedia.org/enwiki/ you will need to download both files and extract the index but not the dump and enter the correct paths for the library to find the files. 

The test project can be run without using the data dump as all of the index and page contents are created in memory.

This library does not parse the XML page elements instead it just returns an object containing the XML. There are other projects on GitHub for parsing the XML.

# Example

```cs
var pageTitles = new List<string>
{
    "Software development",
    "Microsoft Visual Studio",
    "JavaScript"
};

using (var indexSearcher = new PageIndexSearcher(@"C:\Data\enwiki-20180701-pages-articles-multistream-index.txt"))
{
    var pageIndexItems = indexSearcher.Search(pageTitles);

    using(var dataDumpReader = new DataDumpReader(@"C:\Data\enwiki-20180701-pages-articles-multistream.xml.bz2"))
    {
        var results = dataDumpReader.Search(pageIndexItems);
    }           
}
