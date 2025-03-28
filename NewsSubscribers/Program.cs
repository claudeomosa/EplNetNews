// See https://aka.ms/new-console-template for more information

using NewsSubscribers;

Console.WriteLine("Hello, World!");

var subscriber = new NewsSubscriber();
await subscriber.Subscribe("127.0.0.1", 8080);