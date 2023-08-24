using SharedLibrary.Configuration;
using SharedLibrary.Data.Content;
using WikiGet.Processing;

namespace WikiGet;


/// <summary>
/// central controller for the WikiGet processes
/// </summary>
public static class WikiGet
{
    public static void ProcessPage(string targetPage, WikiGetLocalConfig config, GlobalConfig globalConfig)
    {
        // performs the complete process for a target page based on configuration
         /*
          * PROCESS:
          * It's actually remarkably simple
          * 1. Download the the page
          * 2. Get the Page from the PageCollection
          * 3. Sanitize the page
          * 4. If the page is to be indexed in the database, send it there.
          *    Otherwise, send it to the Saver to be written to disk.
          * done
          */
         
         PageCollection collection = Downloader.DownloadPages(new []{targetPage}, globalConfig.WikiConnection);
         Page page = collection.First();

         
         page = Sanitizer.Sanitize(page, globalConfig.WikiConnection);
         
         
         if (config.Index) Indexer.IndexPage(page, globalConfig.DatabaseConnection, config, globalConfig);
         else Putter.SendToOutput(page, config);
    }

    public static void Start(ILocalConfig localConfig, GlobalConfig globalConfig)
    {
        if (localConfig is not WikiGetLocalConfig config) throw new Exception("Execution protocol improperly assigned.");
        
        foreach (string target in config.TargetPages)
        {
            ProcessPage(target, config, globalConfig);
        }
    }
}