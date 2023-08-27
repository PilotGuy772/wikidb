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
    }
}