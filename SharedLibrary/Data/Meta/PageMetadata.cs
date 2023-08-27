using System.Xml;
using SharedLibrary.Configuration;
using SharedLibrary.Configuration.Connections;
using SharedLibrary.Data.Content;

namespace SharedLibrary.Data.Meta;


/// <summary>
/// Get pages by name, wiki, & DB.
/// </summary>
public static class PageMetadata
{
    //handles getting pages from a database by name & wiki
    
    
    public static Page GetPage(string title, GlobalConfig config)
    {
        /*
         * XML Schema:
         *
         * <page>
                <name>page</name>
                <parent></parent>
                <url>https://wiki.archlinux.org/title/page</url>
                <path>{}/pages/archwiki/page.html</path>
                <children>
                    <child>subpage</child>
                </children>
         * </page>
         *  
         */
        
        var pathToMetadata = $"{config.DatabaseConnection.Path}/pages/{config.WikiConnection.Name}/{title}.html.meta";

        XmlDocument doc = new();
        doc.Load(pathToMetadata);
        
        XmlNode pageNode = doc.GetElementsByTagName("page")[0] ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        string name = pageNode["name"]?.InnerText ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        string parent = pageNode["parent"]?.InnerText ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        string url = pageNode["url"]?.InnerText ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        string path = pageNode["path"]?.InnerText.Replace("{}", config.DatabaseConnection.Path) ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        string[] children = pageNode["children"]?.ChildNodes.Cast<XmlNode>().Select(x => x.InnerText).ToArray() ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        IEnumerable<Page> childPages = children.Select(childName => GetPage(childName, config));
        //get content
        string content = File.ReadAllText(path);
        //this is skipped in the nearly identical method GetPageWithoutContent.
        
        return new Page(childPages, name, content, null, config.WikiConnection.Name, url, path);
    }

    public static Page GetPageWithoutContent(string title, GlobalConfig config)
    {
        string pathToMetadata =
            Path.Combine(config.DatabaseConnection.Path.Replace("~", Environment.GetEnvironmentVariable("HOME")),
                "pages", config.WikiConnection.Name, title + ".html.xml");

        XmlDocument doc = new();
        doc.Load(pathToMetadata);
        
        XmlNode pageNode = doc.GetElementsByTagName("page")[0] ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        string name = pageNode["name"]?.InnerText ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        string parent = pageNode["parent"]?.InnerText ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        string url = pageNode["url"]?.InnerText ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        string path = pageNode["path"]?.InnerText.Replace("{}", config.DatabaseConnection.Path) ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        string[] children = pageNode["children"]?.ChildNodes.Cast<XmlNode>().Select(x => x.InnerText).ToArray() ?? throw new InvalidDataException("A page metadata file is malformed. Affected page: " + title);
        IEnumerable<Page> childPages = children.Select(x => GetPage(x, config));

        return new Page(childPages, name, $"<h2>{name}</h2><br/><p style=\"font-family: monospace\">For some reason, the actual content of the requested page was not retrieved.</p>", null, config.WikiConnection.Name, url, path);
    }
}