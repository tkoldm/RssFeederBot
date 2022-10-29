namespace Domain.Models;

public class ParsedModel
{
    /// <summary>
    /// Идентификатор новости
    /// </summary>
    public Guid Identifier { get; }

    /// <summary>
    /// Заголовок статьи
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Короткое описание
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Ссылка на источник
    /// </summary>
    public string OriginalLink { get; }

    /// <summary>
    /// Данные полученные из внешнего ресурса
    /// </summary>
    /// <param name="title">Заголовок</param>
    /// <param name="description">Описание</param>
    /// <param name="originalLink">Ссылка на оригинальную статью</param>
    public ParsedModel(string title, string description, string originalLink)
    {
        Identifier = Guid.NewGuid();
        Title = title;
        Description = description;
        OriginalLink = originalLink;
    }

    public override string ToString()
    {
        return $"{Title}\n{Description.Substring(0, Math.Min(Description.Length - 1, 2000))}\n{OriginalLink}\n\n";
    }
}