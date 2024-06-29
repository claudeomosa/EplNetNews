namespace NewsScraper;
using HtmlAgilityPack;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
public static class Spider
{
    public static void Init()
    {
        Console.WriteLine("Spider started Scraping");
        string url = "https://www.skysports.com/premier-league-news";
        List<string> newsSnippets = GetNewsSnippets(url);
        WriteToPdf(newsSnippets);
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

    private static void WriteToPdf(List<string> newsSnippets)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        string currentDirectory = Directory.GetCurrentDirectory();
        string projectRootDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName).FullName;
        string outputFileName = "Snippets_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".pdf";
        string outputPath = Path.Combine(projectRootDirectory, outputFileName);

        if (!Directory.Exists(projectRootDirectory))
        {
            Directory.CreateDirectory(projectRootDirectory);
        }

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(20));

                page.Header()
                    .Text("Football Snippets Received At: " + DateTime.Now)
                    .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);
                        foreach (var snip in newsSnippets)
                        {
                            x.Item().Text(snip);
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });

            });
        });
        document.GeneratePdf(outputPath);
    }
}
