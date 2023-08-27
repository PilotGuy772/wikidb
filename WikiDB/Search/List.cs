namespace WikiDB.Search;

/// <summary>
/// Handles listing of databases, wikis, and pages. This is the controller for the list command.
/// </summary>
public static class List
{
    public static void ListMode(WikiDBLocalConfig config)
    {
        switch (config)
        {
            // if the only listed options are -l (besides -i or -c), go to TreeSearch.
            case
            {
                ListMode: true, DatabaseListMode: false, PageListMode: false, WikiListMode: false, TargetDatabase: null,
                TargetWiki: null
            }:
                TreeSearch.ShowTree(config);
                break;
            // if the only listed options are -l and -d, go to SearchItems.
            case
            {
                ListMode: true, DatabaseListMode: true, PageListMode: false, WikiListMode: false, TargetDatabase: null,
                TargetWiki: null
            }:
                SearchItems.ShowDatabases(config);
                break;
            // if the only listed options are -l and -w, go to SearchItems.
            case
            {
                ListMode: true, DatabaseListMode: false, PageListMode: false, WikiListMode: true, TargetDatabase: null,
                TargetWiki: null
            }:
                SearchItems.ShowWikis(config);
                break;
            //if the only listed options are -l and -p, go to SearchItems
            case
            {
                ListMode: true, DatabaseListMode: false, PageListMode: true, WikiListMode: false, TargetDatabase: null,
                TargetWiki: null
            }:
                SearchItems.ShowPages(config);
                break;
            //specific list mode: if -D is specified, go to SpecificList.
            case
            {
                ListMode: true, DatabaseListMode: false, PageListMode: false, WikiListMode: false, TargetDatabase: not null,
                TargetWiki: null
            }:
                SpecificList.ListWikisInDatabase(config);
                break;
            
            //specific list mode: if -W AND -D is specified, go to SpecificList.
            case
            {
                ListMode: true, DatabaseListMode: false, PageListMode: false, WikiListMode: false, TargetDatabase: not null,
                TargetWiki: not null
            }:
                SpecificList.ListPagesInDatabaseAndWiki(config);
                break;
            
            //specific list mode: if -W is specified, go to SpecificList.
            case
            {
                ListMode: true, DatabaseListMode: false, PageListMode: false, WikiListMode: false, TargetDatabase: null,
                TargetWiki: not null
            }:
                SpecificList.ListPagesInWiki(config);
                break;
        }
    }
}