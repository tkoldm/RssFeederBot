// See https://aka.ms/new-console-template for more information

using Domain.Services.Abstractions;
using Domain.Settings;
using Infrastructure.Bot;
using Infrastructure.MessageBroker;
using Infrastructure.RssParser;
using Infrastructure.UserStore;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Polling;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var token = config.GetSection("TelegramToken").Value;

IUserStore userStore = new UserStore();

Bot bot = new(token, userStore);
IParser rssParser = new RssParser();/*new[]
{
    "https://habr.com/ru/rss/feed/posts/all/fa8ae28a52672f5463cc1cc0d4deef11/?fl=ru",
    "https://jobs.dou.ua/vacancies/feeds/?category=.NET"
});*/


Broker broker = new(bot, rssParser, userStore);

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
Console.ReadLine();