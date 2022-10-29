using System.Xml;
using Domain.Extensions;
using Domain.Models;
using Domain.Services.Abstractions;
using Infrastructure.MessageBroker;

namespace Infrastructure.RssParser;

public class RssParser : IParser
{
    private readonly string[] _rssLinks;
    private readonly Broker _broker;

    public RssParser(string[] rssLinks, Broker broker)
    {
        _rssLinks = rssLinks;
        _broker = broker;
    }

    public async Task ParseAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            foreach (var rssLink in _rssLinks)
            {
                var client = new HttpClient();
                var responseMessage = await client.GetAsync(rssLink);
                var content = await responseMessage.Content.ReadAsStringAsync();
                var currentLinkFeeds = ParseRssFeed(content);
                if (currentLinkFeeds != null && currentLinkFeeds.Any())
                {
                    _broker.Produce(currentLinkFeeds);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            Thread.Sleep(TimeSpan.FromSeconds(600));
        }
    }

    private ParsedModel[]? ParseRssFeed(string xml)
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

            feeds.Add(new ParsedModel(title, description, link));
        }

        return feeds.ToArray();
    }
}