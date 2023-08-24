using System.Xml;
using SharedLibrary.Application.Logging;
using SharedLibrary.Configuration.Connections;

namespace SharedLibrary.Configuration;

/// <summary>
/// Handles configuration for the application. This class cannot be inherited. An instance of this class stores values for the current instance of the application. The basic config class only stores data gleaned from the config files.
/// The data in this class is used for all application instances, while other configuration classes are specific to the application.
/// </summary>
public class GlobalConfig
{
    //database connection  
    public DatabaseConnection DatabaseConnection { get; private set; }
    
    //wiki connection
    public WikiConnection WikiConnection { get; private set; }

    private GlobalConfig(DatabaseConnection databaseConnection, WikiConnection wikiConnection)
    {
        DatabaseConnection = databaseConnection;
        WikiConnection = wikiConnection;
    }

    public static IEnumerable<DatabaseConnection> GetAllDatabases()
    {
        string pathToConfigFile;
        if (File.Exists("~/.config/wikidb/config.xml"))
        {
            pathToConfigFile = "~/.config/wikidb/config.xml";
        }
        else if(File.Exists("/etc/wikidb/config.xml"))
        {
            pathToConfigFile = "/etc/wikidb/config.xml";
        }
        else
            throw new FileNotFoundException("Could not find config file.");
        
        XmlDocument doc = new();
        doc.Load(pathToConfigFile);
        //remove comments cuz I guess System.Xml doesn't do that for you
        XmlNodeList? comments = doc.SelectNodes("//comment()");
        //remove all comments
        if (comments != null)
            foreach (XmlNode comment in comments)
                comment.ParentNode?.RemoveChild(comment);

        XmlNodeList databaseNodes = doc.GetElementsByTagName("database");
        return from XmlNode databaseNode in databaseNodes
            let name =
                databaseNode["name"]?.InnerText ??
                throw new InvalidDataException("The configuration file is malformed.")
            let root = 
                databaseNode["root"]?.InnerText ??
                throw new InvalidDataException("The configuration file is malformed.")
            select new DatabaseConnection(name, root);
    }

    public static GlobalConfig ReadFromConfigFile(string? db = null, string? wiki = null)
    {
        string pathToConfigFile;
        if (File.Exists(Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? string.Empty, ".config/wikidb/config.xml")))
        {
            pathToConfigFile = Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? string.Empty, ".config/wikidb/config.xml");
        }
        else if(File.Exists("/etc/wikidb/config.xml"))
        {
            pathToConfigFile = "/etc/wikidb/config.xml";
        }
        else
            throw new FileNotFoundException("Could not find config file.");
        
        XmlDocument doc = new();
        doc.Load(pathToConfigFile);
        
        //remove comments cuz I guess System.Xml doesn't do that for you
        XmlNodeList? comments = doc.SelectNodes("//comment()");
        //remove all comments
        if (comments != null)
            foreach (XmlNode comment in comments)
                comment.ParentNode?.RemoveChild(comment);
        
        // database connections
        /*
         * First, get the complete list of databases configured in the config file.
         * Then, get the default database.
         *
         * Format for this part of XML:
         * 
         * <databases>
         *      <database>
         *          <name>main</name>
         *          <root>~/.local/share/wikidb</root>
         *     </database>
         * </databases>
         *
         * <defaults>
         *     <databaseDefault>main</databaseDefault>
         *     <wikiDefault>archwiki</wikiDefault>
         * </defaults>
         */
        
        XmlNodeList databaseNodes = doc.GetElementsByTagName("database");
        List<DatabaseConnection> databaseConnections = (from XmlNode databaseNode in databaseNodes
            let name =
                databaseNode["name"]?.InnerText ??
                throw new InvalidDataException("The configuration file is malformed.")
            let root = 
                databaseNode["root"]?.InnerText ??
                throw new InvalidDataException("The configuration file is malformed.")
            select new DatabaseConnection(name, root)).ToList();
        
        XmlNodeList defaultNodes = doc.GetElementsByTagName("databaseDefault");
        string defaultDatabaseName = defaultNodes[0]?.InnerText ?? databaseConnections[0].Name; // in case a default is not specified, use the first in the list.
        DatabaseConnection defaultDatabase = 
            databaseConnections.Find(db == null  
                ? database => database.Name == defaultDatabaseName 
                : database => database.Name == db) 
            ?? throw new InvalidDataException("The requested database is not configured in the config file.");
        
        // wiki connections
        /*
         * First, get the complete list of wikis configured in the config file.
         * Then, get the default wiki.
         *
         * Format for this part of XML:
         *
         * <wikis>
                <wiki>
                    <name>archwiki</name>
                    <url>https://wiki.archlinux.org/title/{}</url>
                    <downloadReferencedImage>false</downloadReferencedImage>
                    
                    <injections>
                    
                        <inject>
                            <destination>/head</destination>

                            <path>/etc/wikidb/wikis/archwiki/style</path>
                            <place>before</place>
                        </inject>

                        <inject>
                            <destination>/body</destination>
                            <path>/etc/wikidb/wikis/archwiki/header</path>
                            <place>before</place>
                        </inject>
                        
                    </injections>

                    <removals>
                    
                        <remove>
                            <id>archnavbar</id>
                        </remove>
                        
                        <remove>
                            <id>mw-sidebar-button p-search p-vector-user-menu-overflow p-personal mw-sidebar-checkbox mw-panel-toc mw-navigation vector-toc-collapsed-checkbox p-lang-btn left-navigation right-navigation catlinks</id>
                        </remove>
                        
                        <remove>
                            <tag>script</tag>
                        </remove>

                    </removals>
                </wiki>
            </wikis>
         */
        
        XmlNodeList wikiNodes = doc.GetElementsByTagName("wiki");
        List<WikiConnection> wikiConnections = (from XmlNode wikiNode in wikiNodes
            let name =
                wikiNode["name"]?.InnerText ??
                throw new InvalidDataException("The configuration file is malformed.")
                
            let url =
                wikiNode["url"]?.InnerText ??
                throw new InvalidDataException("The configuration file is malformed.")
                
            let downloadReferencedImage =
                bool.Parse(wikiNode["downloadReferencedImage"]?.InnerText ?? "false")
                
            let injections = (from XmlNode injectionNode in wikiNode["injections"]?.ChildNodes ?? null //injections are not mandatory
                let destination =
                    injectionNode["destination"]?.InnerText ??
                    throw new InvalidDataException("The configuration file is malformed.")
                let path =
                    injectionNode["path"]?.InnerText ??
                    throw new InvalidDataException("The configuration file is malformed.")
                let place =
                    injectionNode["place"]?.InnerText == "before" //default is after. If an invalid value is specified, default.
                select new Injection(destination, path, place)).ToList()
            
            let removals = (from XmlNode removalNode in wikiNode["removals"]?.ChildNodes ?? null       //removals are not mandatory
                let type = removalNode.ChildNodes[0].Name switch
                {
                    "id" => RemovalType.Id,
                    "xpath" => RemovalType.Xpath,
                    "tag" => RemovalType.TagType,
                    _ => throw new InvalidDataException("The configuration file is malformed. thing: " + removalNode.ChildNodes[0].Name)
                }
                let value =
                    removalNode.ChildNodes[0].InnerText ??
                    throw new InvalidDataException("The configuration file is malformed.")
                select new Removal(type, value)).ToList()
            
            select new WikiConnection(name, url, downloadReferencedImage, injections, removals)).ToList();
        
        
        XmlNodeList defaultWikiNodes = doc.GetElementsByTagName("wikiDefault");
        WikiConnection defaultWiki = 
            wikiConnections.Find(wiki == null  
                ? fwiki => fwiki.Name == defaultWikiNodes[0]?.InnerText
                : fwiki => fwiki.Name == wiki) 
            ?? throw new InvalidDataException("The requested wiki is not configured in the config file."); 
        
        return new GlobalConfig(defaultDatabase, defaultWiki);
    }

}