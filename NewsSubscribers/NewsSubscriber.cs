using ThePress;

namespace NewsSubscribers;

using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

public class NewsSubscriber
{
    public async Task Subscribe(string serverIp, int port)
    {
        var client = new TcpClient();
        await client.ConnectAsync(serverIp, port);
        
        Console.WriteLine("Connected to publisher. Waiting for news...");
        
        var stream = client.GetStream();
        var buffer = new byte[1024 * 1024]; // 1MB buffer
        
        while (true)
        {
            try
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;
                
                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var broadcast = JsonConvert.DeserializeObject<BroadcastMessage>(message);
                
                Console.WriteLine($"Received news: {broadcast.Title}");
                Console.WriteLine($"PDF size: {broadcast.PdfSize} bytes");
                
                // Save PDF to file
                var pdfBytes = Convert.FromBase64String(broadcast.PdfData);
                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{broadcast.Title.Substring(0, Math.Min(20, broadcast.Title.Length))}.pdf";
                File.WriteAllBytes(fileName, pdfBytes);
                
                Console.WriteLine($"Saved to {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                break;
            }
        }
        
        client.Close();
    }
}

// To use the subscriber:
// var subscriber = new NewsSubscriber();
// await subscriber.Subscribe("127.0.0.1", 8080);