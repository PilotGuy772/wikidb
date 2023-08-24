using System.Xml;
using SharedLibrary.Application.Logging;
using SharedLibrary.Configuration;
using SharedLibrary.Configuration.Connections;
using SharedLibrary.Data.Content;

namespace SharedLibrary.Data.Meta;


/// <summary>
/// Get basic database information from a database meta file. This class also has utilities to get PageCollections from the database.
/// </summary>
public class DatabaseMetadata
{
    //retrieves database information from a database meta file. This class may be optionally used to store database information in memory if the task requires it.
    
    /*
     * XML Format:
     *
     * <database>
            <name>main</name>

            <wiks>
                <wiki>
                    <name>archwiki</name>

                    <pages>
                        <page>page</page>
                    </pages>
                    
                </wiki>
            </wiks>

            <pageCounter>1</pageCounter>

     *  </database>
     * 
     */

    public static IEnumerable<WikiReference> GetBasicMetadata(DatabaseConnection connection)
    {
        XmlDocument doc = new();
        doc.Load($"{connection.Path}/database.xml"); //all metadata files keep the .xml extension
        
        XmlNode databaseNode = doc.GetElementsByTagName("database")[0] ?? throw new InvalidDataException("A database metadata file is malformed. Affected database: \"" + connection.Name + "\"");
        string name = databaseNode["name"]?.InnerText ?? throw new InvalidDataException("A database metadata file is malformed. Affected database: \"" + connection.Name + "\"");
        if (!name.Equals(connection.Name))
            Logger.Log("The database name in the metadata file does not match the database name in the config file. This may cause issues.", InfoTier.Warning);

        XmlNodeList wikiNodes = databaseNode["wikis"]?.ChildNodes ?? throw new InvalidDataException("A database metadata file is malformed. Affected database: \"" + connection.Name + "\"");
        
        return
            (from XmlNode wikiNode in wikiNodes 
                let wikiName = wikiNode["name"]?
                                    .InnerText 
                            ?? throw new InvalidDataException("A database metadata file is malformed. Affected database: \"" + connection.Name + "\"") 
                let pages = wikiNode["pages"]?
                                    .ChildNodes
                                    .Cast<XmlNode>()
                                    .Select(x => x.InnerText)
                                    .ToArray() 
                            ?? throw new InvalidDataException("A database metadata file is malformed. Affected database: \"" + connection.Name + "\"") 
                select new WikiReference(wikiName, pages)).ToArray();
    }
    
    
    public static PageCollection GetPageCollection(GlobalConfig config) => 
        new PageCollection(
            GetBasicMetadata(config.DatabaseConnection)
                .First(x => x.Name.Equals(config.WikiConnection.Name))//choose appropriate wiki
                .Pages//get all pages
                .Where(x => !x.Contains('/'))//filter out child pages
                .Select(x => PageMetadata.GetPage(x, config)));//get the entire page for each
    
        //return a complete PageCollection representing all pages in the specified wiki of the connected database.
        /*
         * PROCESS:
         * 1. retrieve a WikiReference from GetBasicMetadata corresponding to the WikiConnection's name.
         * 2. for each page in the WikiReference that IS NOT a child page (i.e. does not have a '/' in the name), get the page using PageMetadata.GetPage() and add it to an IEnumerable<Page>.
         * 3. return a new PageCollection from the IEnumerable<Page>.
         */
        
    public static PageCollection GetPageCollectionWithoutContent(GlobalConfig config) => 
        new PageCollection(
            GetBasicMetadata(config.DatabaseConnection)
                .First(x => x.Name.Equals(config.WikiConnection.Name))//choose appropriate wiki
                .Pages//get all pages
                .Where(x => !x.Contains('/'))//filter out child pages
                .Select(x => PageMetadata.GetPageWithoutContent(x, config)));//get the entire page for each except for content
    
    
}