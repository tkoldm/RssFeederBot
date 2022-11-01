using System.Xml;
using Domain.Extensions;
using Domain.Models;
using Domain.Services.Abstractions;
using Infrastructure.MessageBroker;

namespace Infrastructure.RssParser;

public class RssParser : IParser, IDisposable
{
    private readonly LinksStorage _linksStorage;
    private readonly ParserBroker _broker;
    private readonly Thread _worker;
    public RssParser(LinksStorage linksStorage, ParserBroker broker)
    {
        _linksStorage = linksStorage;
        _broker = broker;
        _worker = new Thread(Consume);
        _worker.Start();
    }

    private async void Consume()
    {
        while (true)
        {
            var links = _linksStorage.GetLinks();
            await ParseAsync(links);
        }
    }
    
    public async Task ParseAsync(IEnumerable<string> urls, CancellationToken cancellationToken = default)
    {
        foreach (var rssLink in urls)
        {
            var client = new HttpClient();
            var responseMessage = await client.GetAsync(rssLink, cancellationToken);
            var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            var chatId = _linksStorage.GetUsersByLink(rssLink);
            var currentLinkFeeds = ParseRssFeed(content, chatId);
            if (currentLinkFeeds != null && currentLinkFeeds.Any())
            {
                _broker.Produce(currentLinkFeeds);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private List<ParsedModel>? ParseRssFeed(string xml, long[] chatIds)
    {
        var rssXml = new XmlDocument();
        rssXml.LoadXml(xml);
        var nodes = rssXml.SelectNodes("rss/channel/item");
        if (nodes == null)
        {
            return null;
        }

        var feeds = new List<ParsedModel>();
        foreach (XmlNode node in nodes)
        {
            if (node == null)
            {
                continue;
            }

            var rssSubNode = node.SelectSingleNode("title");
            var title = rssSubNode != null ? rssSubNode.InnerText : "";

            rssSubNode = node.SelectSingleNode("link");
            var link = rssSubNode != null ? rssSubNode.InnerText : "";

            rssSubNode = node.SelectSingleNode("description");
            var description = rssSubNode != null
                ? rssSubNode.InnerText.TrimHtmlTags().TrimString()
                : "";

            feeds.Add(new ParsedModel(title, description, link, chatIds));
        }

        return feeds;
    }

    public void Dispose()
    {
        _worker.Interrupt();
    }
}