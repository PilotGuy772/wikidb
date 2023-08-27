using SharedLibrary.Configuration;
using SharedLibrary.Configuration.Connections;
using SharedLibrary.Data.Content;
using SharedLibrary.Data.Meta;

namespace WikiDB.Search;

internal static class SpecificList
{
    
    /// <summary>
    /// List the wikis in the specified database.
    /// </summary>
    /// <param name="config">Local config</param>
    public static void ListWikisInDatabase(WikiDBLocalConfig config)
    {
        /*
         * PROCESS:
         * 0. Get the requested columns.
         * 1. Get all wikis.
         * 2. Iterate through each wiki.
         *    2.1. Print the requested information about the wiki.
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
        
        
        //get all wikis
        IEnumerable<WikiReference> wikiReferences = DatabaseMetadata.GetBasicMetadata(GlobalConfig
            .GetAllDatabases().First(x => x.Name.Equals(config.TargetDatabase)));

        IEnumerable<WikiConnection> wikis = GlobalConfig.GetAllWikis().Where(x => DatabaseMetadata.GetBasicMetadata(GlobalConfig
            .GetAllDatabases().First(z => z.Name.Equals(config.TargetDatabase))).Select(y => y.Name).Contains(x.Name)).OrderBy(x => x.Name);
        
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

    /// <summary>
    /// List all pages in the given database and wiki
    /// </summary>
    /// <param name="config">Local config</param>
    public static void ListPagesInDatabaseAndWiki(WikiDBLocalConfig config)
    {
        /*
         * PROCESS:
         * 0. Get the requested columns.
         * 1. Get all pages from the requested wiki/database
         * 2. Iterate through each page.
         *   2.1. Print the requested information about the page.
         */
        
        //get the requested columns
        //this is essentially just a cut-and-paste from TreeSearch.ShowTree.
        List<string> requestedColumns = new();
        if (config.Columns != null)
            requestedColumns.AddRange(config.Columns);
        else if (config.InfoMode)
            requestedColumns.AddRange(new[] { "name", "url", "path" });
        else
            requestedColumns.Add("name");
        
        //get the database and wiki connections corresponding to the requested database and wiki
        DatabaseConnection database = GlobalConfig.GetAllDatabases().First(x => x.Name.Equals(config.TargetDatabase));

        //get all pages
        IEnumerable<Page> pages = DatabaseMetadata.GetBasicMetadata(database)
            .FirstOrDefault(wr => wr.Name.Equals(config.TargetWiki ?? throw new Exception("very impressive.")))
            .Pages
            .Select(name => 
                PageMetadata.GetPageWithoutContent(
                    name, database, config.TargetWiki ?? throw new Exception("very impressive.")));
        
        //iterate through each page
        foreach (Page page in pages)
        {
            //so as to aid with grepping and other stuff, ONLY print the requested column(s) for each page.
            foreach (string column in requestedColumns)
            {
                switch (column)
                {
                    case "name":
                        Console.Write(page.Name);
                        break;
                    case "url":
                        Console.Write(page.Url);
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

    /// <summary>
    /// List all pages across all databases in the specified wiki. This does not check for nor account for duplicate pages.
    /// </summary>
    /// <param name="config">Local config</param>
    public static void ListPagesInWiki(WikiDBLocalConfig config)
    {
        /*
         * PROCESS:
         * 0. Get the requested columns.
         * 1. Get all pages.
         * 2. Filter the ones that are not members of the requested wiki
         * 3. Iterate through each page.
         *      3.1. Print the requested information about the page.
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
        /*IEnumerable<Page> pages = GlobalConfig.GetAllDatabases()
            .SelectMany(database => DatabaseMetadata.GetBasicMetadata(database)
                .SelectMany(wiki => wiki.Pages))
            .Select(name => PageMetadata.GetPageWithoutContent(name, GlobalConfig.GetAllDatabases()
                .First(x => x.Name.Equals(config.TargetDatabase)), config.TargetWiki ?? throw new Exception("very impressive.")));*/

        IEnumerable<Page> pages = GlobalConfig.GetAllDatabases().SelectMany(db => DatabaseMetadata.GetBasicMetadata(db)
            .First(wr => wr.Name.Equals(config.TargetWiki))
            .Pages
            .Select(name => PageMetadata.GetPageWithoutContent(name, db,
                config.TargetWiki ?? throw new Exception("very impressive, buckaroo."))));
        
        //iterate and print
        foreach (Page page in pages)
        {
            //so as to aid with grepping and other stuff, ONLY print the requested column(s) for each page.
            foreach (string column in requestedColumns)
            {
                switch (column)
                {
                    case "name":
                        Console.Write(page.Name);
                        break;
                    case "url":
                        Console.Write(page.Url);
                        break;
                    case "path":
                        Console.Write(page.Path);
                        break;
                    case "wiki":
                        Console.Write(page.Wiki);
                        break;
                }
                Console.Write("  ");
            }
            
            Console.WriteLine();
        }

    }
}