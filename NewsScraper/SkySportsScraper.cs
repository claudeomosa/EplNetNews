using HtmlAgilityPack;
using System.Net;

namespace NewsScraper;

public class SkySportsScraper
{
    private const string SkySportsEPLUrl = "https://www.skysports.com/premier-league-news";
    
    public List<NewsArticle> GetLatestNews()
    {
        var articles = new List<NewsArticle>();
        
        try
        {
            var web = new HtmlWeb();
            var doc = web.Load(SkySportsEPLUrl);
            
            var newsNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'news-list__item')]");
            
            foreach (var node in newsNodes)
            {
                var title = node.SelectSingleNode(".//a[contains(@class, 'news-list__headline-link')]")?.InnerText.Trim();
                var url = node.SelectSingleNode(".//a[contains(@class, 'news-list__headline-link')]")?.GetAttributeValue("href", "");
                var summary = node.SelectSingleNode(".//p[contains(@class, 'news-list__snippet')]")?.InnerText.Trim() ?? string.Empty;
                var date = node.SelectSingleNode(".//span[contains(@class, 'label__timestamp')]")?.InnerText.Trim() ?? string.Empty;
                
                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(url))
                {
                    articles.Add(new NewsArticle
                    {
                        Title = WebUtility.HtmlDecode(title),
                        Url = new Uri(new Uri(SkySportsEPLUrl), url).AbsoluteUri,
                        Summary = WebUtility.HtmlDecode(summary),
                        Date = date,
                        Content = GetArticleContent(new Uri(new Uri(SkySportsEPLUrl), url).AbsoluteUri)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Scraping error: {ex.Message}");
        }
        
        return articles;
    }
    
    private string GetArticleContent(string articleUrl)
    {
        try
        {
            var web = new HtmlWeb();
            var doc = web.Load(articleUrl);
            var contentNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article__body')]//p");
            
            return string.Join("\n", contentNodes.Select(n => WebUtility.HtmlDecode(n.InnerText.Trim())));
        }
        catch
        {
            return string.Empty;
        }
    }
}

public class NewsArticle
{
    public string Title { get; set; }
    public string Url { get; set; }
    public string Summary { get; set; }
    public string Date { get; set; }
    public string Content { get; set; }
}