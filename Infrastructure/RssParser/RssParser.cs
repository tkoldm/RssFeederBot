using System.Xml;
using Domain.Extensions;
using Domain.Models;
using Domain.Services.Abstractions;

namespace Infrastructure.RssParser;

public class RssParser : IParser
{
    public async Task<IEnumerable<ParsedModel>> ParseAsync(IEnumerable<string> urls, long chatId, CancellationToken cancellationToken = default)
    {
        var models = new List<ParsedModel>();
        foreach (var rssLink in urls)
        {
            var client = new HttpClient();
            var responseMessage = await client.GetAsync(rssLink, cancellationToken);
            var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            var currentLinkFeeds = ParseRssFeed(content, chatId);
            if (currentLinkFeeds != null && currentLinkFeeds.Any())
            {
                models.AddRange(currentLinkFeeds);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return models;
    }

    private ParsedModel[]? ParseRssFeed(string xml, long chatId)
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

            feeds.Add(new ParsedModel(title, description, link, chatId));
        }

        return feeds.ToArray();
    }
}