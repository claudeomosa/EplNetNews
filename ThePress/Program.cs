using NewsScraper;
using ThePress;

SkySportsScraper _scraper = new SkySportsScraper();
NewsPublisher _publisher;
Timer _scrapingTimer;

Console.WriteLine("SkySports EPL News Publisher");
        
// Initialize publisher on port 8080
_publisher = new NewsPublisher(8080);
_publisher.Start();
        
// Set up timer to scrape every hour
_scrapingTimer = new Timer(ScrapeAndPublish, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        
Console.WriteLine("Press any key to stop the publisher...");
Console.ReadKey();
        
_scrapingTimer.Dispose();
Console.WriteLine("Publisher stopped.");

async void ScrapeAndPublish(object state)
{
    Console.WriteLine($"Scraping SkySports at {DateTime.Now}");
        
    var articles = _scraper.GetLatestNews();
    if (articles.Count > 0)
    {
        Console.WriteLine($"Found {articles.Count} articles. Publishing the latest one.");
        await _publisher.BroadcastNews(articles[0]);
    }
    else
    {
        Console.WriteLine("No articles found.");
    }
}