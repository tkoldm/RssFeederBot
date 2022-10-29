// See https://aka.ms/new-console-template for more information

using System.Text;
using Domain.Services.Abstractions;
using Domain.Settings;
using Infrastructure.Bot;
using Infrastructure.MessageBroker;
using Infrastructure.RssParser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Polling;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var appSettings = config.Get<AppSettings>();


Broker broker = new();
Bot bot = new(appSettings.TelegramToken);
IParser rssParser = new RssParser(new[]
{
    "https://habr.com/ru/rss/feed/posts/all/fa8ae28a52672f5463cc1cc0d4deef11/?fl=ru",
    "https://jobs.dou.ua/vacancies/feeds/?category=.NET"
}, broker);
var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }, // receive all update types
};
bot.StartReceiving(
    receiverOptions,
    cancellationToken
);
rssParser.ParseAsync();
Console.ReadLine();