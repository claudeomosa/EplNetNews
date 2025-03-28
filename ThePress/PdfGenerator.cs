using NewsScraper;

namespace ThePress;
using iTextSharp.text;
using iTextSharp.text.pdf;

public class PdfGenerator
{
    public byte[] CreatePdf(NewsArticle article)
    {
        using(var ms = new MemoryStream())
        {
            var document = new Document(PageSize.A4, 50, 50, 25, 25);
            var writer = PdfWriter.GetInstance(document, ms);
            
            document.Open();
            
            // Add title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var titleParagraph = new Paragraph(article.Title, titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f
            };
            document.Add(titleParagraph);
            
            // Add date
            var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            var dateParagraph = new Paragraph(article.Date, dateFont)
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingAfter = 15f
            };
            document.Add(dateParagraph);
            
            // Add content
            var contentFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
            var contentParagraph = new Paragraph(article.Content, contentFont)
            {
                Alignment = Element.ALIGN_LEFT,
                SpacingAfter = 15f
            };
            document.Add(contentParagraph);
            
            // Add URL
            var urlFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10);
            var urlParagraph = new Paragraph($"Source: {article.Url}", urlFont)
            {
                Alignment = Element.ALIGN_RIGHT
            };
            document.Add(urlParagraph);
            
            document.Close();
            return ms.ToArray();
        }
    }
}