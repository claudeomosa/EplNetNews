using System.Net;
using System.Net.Sockets;
using System.Text;
using NewsScraper;
using Newtonsoft.Json;

namespace ThePress;

public class NewsPublisher
{
    private TcpListener _listener;
    private List<TcpClient> _subscribers = new List<TcpClient>();
    private readonly int _port;
    
    public NewsPublisher(int port)
    {
        _port = port;
    }
    
    public void Start()
    {
        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();
        Console.WriteLine($"Publisher started on port {_port}. Waiting for subscribers...");
        
        // Start accepting clients in a separate thread
        Task.Run(() => AcceptClients());
    }
    
    private async void AcceptClients()
    {
        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            lock (_subscribers)
            {
                _subscribers.Add(client);
            }
            Console.WriteLine($"New subscriber connected. Total subscribers: {_subscribers.Count}");
        }
    }
    
    public async Task BroadcastNews(NewsArticle article)
    {
        var pdfGenerator = new PdfGenerator();
        var pdfBytes = pdfGenerator.CreatePdf(article);
        
        var message = new BroadcastMessage
        {
            Title = article.Title,
            PdfSize = pdfBytes.Length,
            PdfData = Convert.ToBase64String(pdfBytes)
        };
        
        var json = JsonConvert.SerializeObject(message);
        var data = Encoding.UTF8.GetBytes(json);
        
        List<TcpClient> disconnectedClients = new List<TcpClient>();
        
        lock (_subscribers)
        {
            foreach (var client in _subscribers)
            {
                try
                {
                    if (client.Connected)
                    {
                        var stream = client.GetStream();
                        stream.WriteAsync(data, 0, data.Length);
                    }
                    else
                    {
                        disconnectedClients.Add(client);
                    }
                }
                catch
                {
                    disconnectedClients.Add(client);
                }
            }
            
            // Remove disconnected clients
            foreach (var dc in disconnectedClients)
            {
                _subscribers.Remove(dc);
                dc.Dispose();
            }
        }
        
        if (disconnectedClients.Count > 0)
        {
            Console.WriteLine($"Removed {disconnectedClients.Count} disconnected subscribers.");
        }
    }
}

public class BroadcastMessage
{
    public string Title { get; set; }
    public int PdfSize { get; set; }
    public string PdfData { get; set; }
}