using System.Text.RegularExpressions;

namespace Domain.Extensions;

public static class StringExtensions
{
    public static string TrimHtmlTags(this string html)
    {
        return Regex.Replace(html, "<.*?>", string.Empty).Replace("Читать далее", string.Empty);
    }

    public static string TrimString(this string input)
    {
        var withoutNewLiners = input.Replace("\n", "");
        var outer = Regex.Replace(withoutNewLiners, @"\s\s+", " ");
        return outer;
    }
}