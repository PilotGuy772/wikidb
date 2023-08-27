using SharedLibrary.Configuration;
using SharedLibrary.Configuration.Connections;
using SharedLibrary.Data.Content;
using SharedLibrary.Data.Meta;

namespace WikiDB.Search;

internal static class SearchItems
{
    /// <summary>
    /// Outputs just the requested columns of the databases.
    /// </summary>
    /// <param name="config">Local configuration</param>
    public static void ShowDatabases(WikiDBLocalConfig config)
    {
        /*
         * PROCESS:
         * 0. Get the requested columns.
         * 1. Get all databases.
         * 2. Iterate through each database.
         *     2.1. Print the requested information about the database.
         */
        
        //get the requested columns
        //this is essentially just a cut-and-paste from TreeSearch.ShowTree.
        List<string> requestedColumns = new();
        if (config.Columns != null)
            requestedColumns.AddRange(config.Columns);
        else if (config.InfoMode)
            requestedColumns.AddRange(new[] { "name", "pageCounter", "url", "wiki", "path" });
        else
            requestedColumns.Add("name");
        
        //get all databases
        IEnumerable<DatabaseConnection> databases = GlobalConfig.GetAllDatabases().OrderBy(x => x.Name);
        
        //iterate through each database
        foreach (DatabaseConnection database in databases)
        {
            //so as to aid with grepping and other stuff, ONLY print the requested column(s) for each database.
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
            
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Outputs just the requested columns of the wikis.
    /// </summary>
    /// <param name="config">Local configuration</param>
    public static void ShowWikis(WikiDBLocalConfig config)
    {
        /*
         * PROCESS:
         * 0. Get the requested columns.
         * 1. Get all wikis as WikiConnections
         * 2. Iterate through each WikiConnection
         *    2.1. Print the requested information about the wiki.
         */
        
        //get the requested columns
        //this is essentially just a cut-and-paste from TreeSearch.ShowTree.
        List<string> requestedColumns = new();
        if (config.Columns != null)
            requestedColumns.AddRange(config.Columns);
        else if (config.InfoMode)
            requestedColumns.AddRange(new[] { "name", "url" });
        else
            requestedColumns.Add("name");
        
        //get all wikis
        IEnumerable<WikiConnection> wikis = GlobalConfig.GetAllWikis().OrderBy(x => x.Name);
        
        //iterate through each wiki
        foreach (WikiConnection wiki in wikis)
        {
            //so as to aid with grepping and other stuff, ONLY print the requested column(s) for each wiki.
            foreach (string column in requestedColumns)
            {
                switch (column)
                {
                    case "name":
                        Console.Write(wiki.Name);
                        break;
                    case "url":
                        Console.Write(wiki.Url);
                        break;
                }
                Console.Write("  ");
            }
            
            Console.WriteLine();
        }
    }

    public static void ShowPages(WikiDBLocalConfig config)
    {
        /*
         * PROCESS:
         * 0. Get the requested columns.
         * 1. Get all pages as PageCollection.
         * 2. Iterate through each page.
         *    2.1. Print the requested information about the page. For the Name, append the name of the parent to the name of the page, if applicable.
         */
        
        //get the requested columns
        //this is essentially just a cut-and-paste from TreeSearch.ShowTree.
        List<string> requestedColumns = new();
        if (config.Columns != null)
            requestedColumns.AddRange(config.Columns);
        else if (config.InfoMode)
            requestedColumns.AddRange(new[] { "name", "url", "wiki", "path" });
        else
            requestedColumns.Add("name");
        
        //get all pages
        //this is more complicated than just... getting all pages
        //we need to get all pages from all wikis from all databases
        
        //Start by getting all databases
        IEnumerable<DatabaseConnection> databases = GlobalConfig.GetAllDatabases();
        
        //then, we can get WikiReference objects from each database
        List<WikiReference> wikiReferences = new();
        IEnumerable<DatabaseConnection> databaseConnections = databases.ToList();
        foreach (DatabaseConnection db in databaseConnections)
        {
            wikiReferences.AddRange(DatabaseMetadata.GetBasicMetadata(db));
        }
        IEnumerable<WikiReference> wikis = wikiReferences;
        
        //then, we can get a list of pages from the list of page names given in each WikiReference
        List<Page> pages = new();
        foreach (WikiReference wiki in wikis)
        {
            pages.AddRange(wiki.Pages.Select(x => PageMetadata.GetPageWithoutContent(x, databaseConnections.First(y => y.Name.Equals(wiki.Database)), wiki.Name)));
        }
        PageCollection pageCollection = new(pages.OrderBy(x => x.Name));
        
        //now we can finally iterate through the pages
        
        //iterate through each page
        foreach (Page page in pageCollection)
        {
            //so as to aid with grepping and other stuff, ONLY print the requested column(s) for each page.
            foreach (string column in requestedColumns)
            {
                switch (column)
                {
                    case "name":
                        Console.Write(page.Parent + page.Name); //add clarity as to whether the requested page is a subpage or not
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
            
            Console.WriteLine();
        }
    }
}