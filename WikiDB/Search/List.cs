namespace WikiDB.Search;

/// <summary>
/// Handles listing of databases, wikis, and pages. This is the controller for the list command.
/// </summary>
public static class List
{
    public static void ListMode(WikiDBLocalConfig config)
    {
        // if the only listed options are -l (besides -i or -c), go to TreeSearch.
        if (config is
            {
                ListMode: true, DatabaseListMode: false, PageListMode: false, WikiListMode: false, TargetDatabase: null,
                TargetWiki: null, TargetPage: null
            })
        {
            TreeSearch.ShowTree(config);
        }
        
        // if the only listed options are -l and -d, go to SearchItems.
        else if (config is
            {
                ListMode: true, DatabaseListMode: true, PageListMode: false, WikiListMode: false, TargetDatabase: null,
                TargetWiki: null, TargetPage: null
            })
        {
            SearchItems.ShowDatabases(config);
        }
        
        // if the only listed options are -l and -w, go to SearchItems.
        else if (config is
            {
                ListMode: true, DatabaseListMode: false, PageListMode: false, WikiListMode: true, TargetDatabase: null,
                TargetWiki: null, TargetPage: null
            })
        {
            SearchItems.ShowWikis(config);
        }
        
        //if the only listed options are -l and -p, go to SearchItems
        else if (config is
            {
                ListMode: true, DatabaseListMode: false, PageListMode: true, WikiListMode: false, TargetDatabase: null,
                TargetWiki: null, TargetPage: null
            })
        {
            SearchItems.ShowPages(config);
        }
    }
}