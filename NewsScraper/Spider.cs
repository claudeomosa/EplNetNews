namespace NewsScraper;
using HtmlAgilityPack;

public static class Spider
{
    public static void Init()
    {
        Console.WriteLine("Spider started Scraping");
        string url = "https://www.skysports.com/premier-league-news";
        List<string> newsSnippets = GetNewsSnippets(url);

        if (newsSnippets.Count > 0)
        {
            foreach (string snippet in newsSnippets)
            {
                Console.WriteLine(snippet);
            }
        }
        else
        {
            Console.WriteLine("No news items found.");
        }
    }

    private static List<string> GetNewsSnippets(string url)
    {
        var web = new HtmlWeb();
        var document = web.Load(url);
        var newsItems = document.DocumentNode
            .SelectNodes("//p")
            .Where(node => node.Attributes["class"]?.Value.Trim() == "news-list__snippet")
            .Take(14)
            .Select(node => node.InnerText.Trim())
            .ToList();

        return newsItems;
    }
}
