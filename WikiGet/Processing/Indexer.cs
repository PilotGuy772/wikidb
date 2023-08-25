using System.Xml;
using SharedLibrary.Application.Logging;
using SharedLibrary.Configuration;
using SharedLibrary.Configuration.Connections;
using SharedLibrary.Data.Content;

namespace WikiGet.Processing;



/// <summary>
/// Handles indexing a retrieved page into a database.
/// </summary>
public static class Indexer
{
    public static void IndexPage(Page page, DatabaseConnection connection, WikiGetLocalConfig localConfig, GlobalConfig globalConfig)
    {
        /*
         * PROCESS:
         * 2. Generate a new XML config file for the page. All of the necessary information should be stored in the Page object itself.
         * 3. If the page is a child page (has a parent), find the parent's config file and update its <children> tag to add self as a child. Only do this if it's not already there.
         * 4. Locate the database's metadata file and update its page counter. Also, index self in the database config file.
         * 5. If the wiki that this page is coming from does not already exist, add a reference to that wiki in the database meta file.
         * 6. Write the page to the appropriate location.
         * 7. Write the XML metadata file to the appropriate location.
         */
        
        /*
         * XML Format:
         * <page>
                <name>page</name>
                <parent/>
                <url>https://wiki.archlinux.org/title/page</url>
                <path>.../pages/archwiki/page.html</path>
                <children>
                    <child>subpage</child>
                </children>
         *  </page>
         */

        if (!File.Exists(Path.Combine(connection.Path, "database.xml")))
        {
            throw new InvalidOperationException("The requested local database has not yet been initialized.");
        }


        XmlDocument pageMetadata = new();
        
        //generate the XML config file
        XmlElement root = pageMetadata.CreateElement("page");
        root.AppendChild(pageMetadata.CreateElement("name"))!.InnerText = page.Title;
        root.AppendChild(pageMetadata.CreateElement("parent"))!.InnerText = page.Parent ?? "";
        root.AppendChild(pageMetadata.CreateElement("url"))!.InnerText = page.Url;
        root.AppendChild(pageMetadata.CreateElement("path"))!.InnerText = page.Path ?? "";
        XmlNode children = root.AppendChild(pageMetadata.CreateElement("children")) ?? throw new NullReferenceException();
        foreach (Page child in page.Children)
        {
            children.PrependChild(pageMetadata.CreateElement("child"))!.InnerText = child.Title;
        }
        
        pageMetadata.AppendChild(root);
        // done
        
        //real quick, we have to generate the path where the page should be stored
        page.Path = Path.Combine(connection.Path, page.Path ?? throw new NullReferenceException("how? how did you manage this?"));
        
        
        //update the parent's config file
        //all we have for the parent is its name, so we have to find its path
        if (page.Parent != null)
        {
            string parentPath = page.Path ?? throw new NullReferenceException(
                "how did this..? how did you do this? seriously. open an issue on GitHub and collect your reward. this should not be possible.");
            parentPath = Path.GetDirectoryName(parentPath) ??
                         throw new NullReferenceException(
                             "how did this even happen? this should not even be possible.");
            parentPath = Path.Combine(parentPath, page.Parent + ".html.xml");


            if (!File.Exists(parentPath))
            {
                Logger.Log(
                    "The downloaded page \"" + page.Title +
                    "\" is a child page, but its parent has not been downloaded yet.", InfoTier.Error);
                Logger.Log("The page cannot be written to the database without its parent.", InfoTier.Warning);
                Logger.Log("Parent page name: " + page.Parent, InfoTier.Warning);
                if (Logger.Prompt("Would you like to download the parent page and try again?", "yes"))
                {
                    /*
                     * Uh-oh... the user wants us to download the parent page.
                     * Remember that the parent page is stored as the ABSOLUTE NAME of the parent, not just the title of the page.
                     * This means that we have the file path already pretty much embedded into the name of the parent.
                     * What's important is that we can just call WikiGet.ProcessPage() again with the parent to download it.
                     * In case the parent of the requested page is also a child page, the user will be prompted for that, too.
                     * This method will just hold until the parent page is done being retrieved.
                     */
                    
                    WikiGet.ProcessPage(page.Parent, localConfig, globalConfig);
                }
                // unfortunately, the parent is mandatory.
                else return;
            }
            
            
            //now edit this XML file to add a new child tag.
            //first, get the entire page into memory
            XmlDocument parentMetadata = new();
            parentMetadata.Load(parentPath);

            //now, make the transformation
            XmlNode childrenNode = parentMetadata.GetElementsByTagName("children")[0] ??
                                   throw new InvalidDataException(
                                       "A page metadata file is malformed. Affected file: " + parentMetadata);
            if (childrenNode.SelectSingleNode("child[text()='" + page.Title + "']") == null)
            {
                childrenNode.AppendChild(parentMetadata.CreateElement("child"))!.InnerText = page.Title;
            }

            //and save to disk
            parentMetadata.Save(parentPath);
            //ezpz}
            
        }

        
        /*
         * XML Format for Database Metadata:
         * <database>
                <name>main</name>
                <wikis>
                    <wiki>
                        <name>archwiki</name>
                        <pages>
                            <page>page</page>
                        </pages>
                    </wiki>
                </wikis>
                <pageCounter>1</pageCounter>
         *  </database>
         */
        
        //update the database's metadata file
        XmlDocument databaseMetadata = new();
        databaseMetadata.Load(Path.Combine(connection.Path, "database.xml"));
        
        XmlNodeList wikis = databaseMetadata.GetElementsByTagName("wiki");
        XmlNode? wikiNode = wikis.Cast<XmlNode>()
            .FirstOrDefault(wiki => 
                wiki.SelectSingleNode("name")?.InnerText == globalConfig.WikiConnection.Name
            );
        //if it's null, we gotta create it
        if (wikiNode == null)
        {
            //find the node named 'wikis'
            XmlNode wikisNode = databaseMetadata.GetElementsByTagName("wikis")[0] ?? throw new InvalidDataException("A database metadata file is malformed. Affected file: " + databaseMetadata);
            //create a new wiki node
            wikiNode = wikisNode.AppendChild(databaseMetadata.CreateElement("wiki")) ?? throw new NullReferenceException();
            //inside the wiki node, create a name node and set its value to the name of the wiki
            wikiNode.AppendChild(databaseMetadata.CreateElement("name"))!.InnerText = globalConfig.WikiConnection.Name;
            //create a new pages node and add the current page to it
            wikiNode.AppendChild(databaseMetadata.CreateElement("pages"));
        }
        
        //check to see if there is already a <page> element with our page name as its value
        //if there is, we don't need to do anything
        //if there isn't, we need to add one
        XmlNodeList pages = databaseMetadata.GetElementsByTagName("page");
        if (pages.Cast<XmlNode>().FirstOrDefault(pageNode => pageNode.InnerText == page.Title) == null)
        {
             //increment the page counter
            XmlNode pageCounter = databaseMetadata.GetElementsByTagName("pageCounter")[0] ?? throw new InvalidDataException("A database metadata file is malformed. Affected file: " + databaseMetadata);
            pageCounter.InnerText = (int.Parse(pageCounter.InnerText) + 1).ToString();
            
            //add self to the database metadata file
            //check if the wiki we are downloading from is indexed in the DB yet

            //if it's not null, we just have to find it and add a reference to the current page to it
            
            
            //first, find the wiki node that we want to add the page to
            //the node should be a 'wiki' type and have a child 'name' node named after the wiki we are downloading from
            //we already have the wiki node, so we just have to find the pages node
            XmlNode pagesNode = wikiNode.SelectSingleNode("pages") ?? throw new InvalidDataException("A database metadata file is malformed. Affected file: " + databaseMetadata);
            //now, add the page to the pages node
            pagesNode.AppendChild(databaseMetadata.CreateElement("page"))!.InnerText = page.Title;
            
            //the database metadata file is now up-to-date
        }
        
       
        
        //now we can write things from memory to disk
        //the first step is to write the page
        if (!Directory.Exists(Path.GetDirectoryName(page.Path))) Directory.CreateDirectory(Path.GetDirectoryName(page.Path) ?? throw new Exception("you failed"));
        File.WriteAllText(page.Path, page.Content);
        
        //next, write the page metadata file
        File.WriteAllText(page.Path + ".xml", pageMetadata.InnerXml);
        
        //then, write the database metadata file
        databaseMetadata.Save(Path.Combine(connection.Path, "database.xml"));
    }
}