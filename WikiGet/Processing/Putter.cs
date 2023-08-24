using SharedLibrary.Data.Content;

namespace WikiGet.Processing;


/// <summary>
/// Handles saving pages to the disk, outputting to STDOUT, or whatever. This class should ONLY be used when not indexing in a database. Use Indexer for that.
/// </summary>
public static class Putter
{
    public static void SendToOutput(Page page, WikiGetLocalConfig config)
    {
        //obligatory copy of the saving logic from WikiGetLocalConfig
        //note about saving pages:
        //when one page is downloaded, it is written to STDOUT.
        //when multiple pages are downloaded, they are written to files in the current directory.
        //when -o is specified for multiple pages, the argument is treated as a directory and pages are written there instead of the current directory.
        //when -o is specified for one page, the argument is treated as a file and the page is written there instead of STDOUT.
        //when a file conflict is detected, the user is prompted to overwrite the file.
        //when -F is specified, the user is not prompted and the file is overwritten.
        //when -f is specified, the output is always sent to STDOUT, even if -o is specified.
        
        //first, see if we have to write to STDOUT
        bool stdout = config.TargetPages.Length == 1 && config.Output == null || config.ForceStdout;

        if (stdout)
        {
            Console.WriteLine("<!-- " + page.Title + " - Downloaded with WikiGet -->\n\n" + page.Content);
            return;
        }

        if (config.TargetPages.Length <= 1) return;
        
        string where = config.Output == null ? "./" : config.Output;
        string path = Path.Combine(where, page.Title + ".html");
        bool allowWrite = config.ForceOverwrite || CheckForFile(path);
                
        if (allowWrite)
            File.WriteAllText(path, page.Content);
    }

    private static bool CheckForFile(string path)
    {
        if (!File.Exists(path)) return true;
        
        Console.WriteLine("The file \"" + path + "\" already exists. Overwrite it? [Y/n] ");
        string input = Console.ReadLine() ?? "";
        
        return input.ToLower() is not ("n" or "no");
    }
}