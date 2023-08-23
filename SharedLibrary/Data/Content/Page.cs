namespace SharedLibrary.Data.Content;

public class Page
{
    public readonly IEnumerable<Page> Children;
    public string Title;
    public string Content;
    public Page? Parent;
    public string Wiki;
    public string Url; //where this page was downloaded from the internet
    public string? Path; //where this page is stored on the local filesystem

    public Page(IEnumerable<Page> children, string title, string content, Page? parent, string wiki, string url, string? path)
    {
        Children = children;
        Title = title;
        Content = content;
        Parent = parent;
        Wiki = wiki;
        Url = url;
        Path = path;
    }

}