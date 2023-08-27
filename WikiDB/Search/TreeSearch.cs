using System.Data;
using SharedLibrary.Configuration;
using SharedLibrary.Configuration.Connections;
using SharedLibrary.Data.Content;
using SharedLibrary.Data.Meta;

namespace WikiDB.Search;


/// <summary>
/// Handles listing pages as trees.
/// </summary>
internal static class TreeSearch
{
    public static void ShowTree(WikiDBLocalConfig config)
    {
        /*
         * PROCESS:
         * 1. Get all pages and databases.
         * 2. Iterate through each database.
         *      2.1. Print the requested information about the database.
         *      2.2. Iterate through each wiki.
         *          2.2.1. Print the requested information about the wiki.
         *          2.2.2. Iterate through each page. and print requested information.
         *          
         */

        IEnumerable<DatabaseConnection> databases = GlobalConfig.GetAllDatabases(); // this only fetches basic DB info from the config, not any metadata
        IEnumerable<WikiConnection> wikis = GlobalConfig.GetAllWikis();
        List<string> requestedColumns = new();
        if (config.Columns != null)
            requestedColumns.AddRange(config.Columns);
        else if (config.InfoMode)
            requestedColumns.AddRange(new[] { "name", "pageCounter", "url", "wiki", "path" });
        else
            requestedColumns.Add("name");

        IEnumerable<DatabaseConnection> databaseConnections = databases.ToList();
        IEnumerable<WikiConnection> wikiConnections = wikis.ToList();
        foreach (DatabaseConnection database in databaseConnections)
        {
            
            bool isLastDatabase = database.Equals(databaseConnections.Last());
            
            //draw a row for the database and its information
            Console.Write("|- ");
            foreach (string column in requestedColumns)
            {
                switch (column)
                {
                    case "name":
                        Console.Write(database.Name);
                        break;
                    case "path":
                        Console.Write(database.Path);
                        break;
                    case "pageCounter":
                        Console.Write(database.PageCounter + " pages");
                        break;
                }
                
                Console.Write("  ");
            }
            
            Console.Write("\n");
            
            
            /*\
             * PROCESS FOR DRAWING WIKIS AND PAGES:
             * Information about a wiki is given when a page that is a member of said wiki is reached.
             * -> Store a list of wiki names representing the wikis that have already been referenced by a page.
             * -> Iterate through every page.
             * -> If this page is a member of a wiki that has not yet been seen, draw the wiki's information and add the name of that wiki to the list of already seen wikis.
             * -> Draw the page's information.
             * -> If the page is a child page, prepend the name of the parent to the name of the page when drawing.
             * -> the end
            \*/
            
            List<string> alreadySeenWikis = new();
            PageCollection pages = DatabaseMetadata.GetPageCollectionWithoutContent(database);

            foreach (Page page in pages)
            {
                WikiConnection wiki = wikiConnections.First(x => page.Wiki.Equals(x.Name));
                bool lastWiki = page.Wiki.Equals(pages.Last().Wiki);
                
                if (!alreadySeenWikis.Contains(page.Wiki))
                {
                    //draw a row for the wiki and its information
                    Console.Write(isLastDatabase ? "    |- " : "|   |- "); //wikis are at one level of indent
                    foreach (string column in requestedColumns)
                    {
                        
                        switch (column)
                        {
                            case "name":
                                Console.Write(wiki.Name);
                                break;
                            case "url":
                                Console.Write(wiki.Url.Replace("{}", "<page_title>"));
                                break;
                        }
                        
                        Console.Write("  ");
                    }
                    
                    Console.Write("\n");
                    
                    alreadySeenWikis.Add(page.Wiki);
                }
                
                //draw a row for the page and its information
                Console.Write((isLastDatabase ? "    " : "|   ") + (lastWiki ? "    |- " : "|   |- ")); //pages are at two levels of indent
                foreach (string column in requestedColumns)
                {
                    switch (column)
                    {
                        case "name":
                            Console.Write(page.Parent == null ? page.Name : Path.Combine(page.Parent, page.Name));
                            break;
                        case "url":
                            Console.Write(page.Url);
                            break;
                        case "wiki":
                            Console.Write(page.Wiki);
                            break;
                        case "path":
                            Console.Write(page.Path);
                            break;
                    }
                    
                    Console.Write("  ");
                }
                
                Console.Write("\n");
            }
            
            alreadySeenWikis.Clear();
        }
    }
}