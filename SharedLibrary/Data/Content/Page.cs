namespace SharedLibrary.Data.Content;

public class Page
{
    public readonly PageCollection Children;
    public readonly string Name;
    public readonly string Content;
    public readonly string? Parent;
    public readonly string Wiki;
    public readonly string Url; //where this page was downloaded from the internet
    public string? Path; //where this page is stored on the local filesystem

    public Page(IEnumerable<Page> children, string name, string content, string? parent, string wiki, string url, string? path)
    {
        Children = new PageCollection(children);
        Name = name;
        Content = content;
        Parent = parent;
        Wiki = wiki;
        Url = url;
        Path = path;
    }

}