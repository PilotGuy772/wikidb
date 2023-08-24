using SharedLibrary.Configuration;

namespace WikiGet;

public class WikiGetLocalConfig : ILocalConfig
{
    //all command line arguments are represented here as public fields.
    //this utility is JUST for downloading pages
    
    // MODES //

    //single download mode: download a single page
    //this is the default mode.

    //recursive download mode: download a page and all pages linked to it.
    //this requires a partner value to represent the maximum depth of recursion.
    //this is controlled by -r or --recursive. If this argument is specified, the program will download the specified page and all pages linked to it.
    //a null value here means recursion should be skipped. A value of 0 means no maximum recursion depth (search the entire tree).
    public int? Recursion { get; private set; }
    
    //search mode: search for a page on the requested wiki.
    //this is controlled by -s or --search. If this argument is specified, the program will search for the specified page on the requested wiki.
    //this is a super complex features that requires a lot of arguments and a lot of logic.
    //If the requested wiki on the internet is a MediaWiki wiki, the program will attempt to use the MediaWiki API to search for the page and return a summary of results.
    //this is controlled by the XML element in the config called "MediaWikiAPI".
    //if the requested wiki is NOT a MediaWiki wiki, the program will attempt to use the search engine specified in the XML element "SearchEngine".
    // this is complicated so I won't implement it yet
    
    //help: whether to print help.
    //this is controlled by -h or --help. If this argument is specified, the program will print help.
    public bool Help { get; private set; }
    
    //index in database: whether to index the downloaded page in the database.
    //this is controlled by -i or --index. If this argument is specified, the program will index the downloaded page in the database.
    public bool Index { get; private set; }
    
    
    // OPTIONS //
    
    //database: the database to use.
    //this is controlled by -D or --database. If this argument is specified, the program will use the specified database.
    //if this argument is specified, the program will index the downloaded page in the specified database.
    //if this argument is not specified, the program will NOT index in any database.
    public string? TargetDatabase { get; private set; }
    
    //wiki: the wiki to use.
    //this is controlled by -W or --wiki. If this argument is specified, the program will use the specified wiki.
    //if this is not specified, the program will use the default wiki.
    public string? TargetWiki { get; private set; }
    
    //verbose: whether to print verbose output.
    //this is controlled by -v or --verbose. If this argument is specified, the program will print verbose output.
    public bool Verbose { get; private set; }

    //page: the page to download.
    //this is controlled by any positional args that are not attached to flags.
    //at least one target page is required.
    public string[] TargetPages { get; private set; } = Array.Empty<string>();
    
    //output: where to output the downloaded pages.
    //by default, the program will print the downloaded pages to the console.
    //this is controlled by -o or --output. If this argument is specified, the program will output the downloaded pages to the specified file.
    public string? Output { get; private set; }

    //markdown: whether to convert downloaded pages to markdown
    //this is controlled by -m or --markdown. If this argument is specified, the program will convert downloaded pages to markdown.
    public bool Markdown { get; private set; }
    
    //force stdout: whether to force output to stdout.
    //when specified, this will always write the output to STDOUT, even if -o is specified.
    //when multiple pages are being downloaded, this will write them all to STDOUT, with each page seperated by special headers.
    //this is controlled by -f or --force-stdout.
    public bool ForceStdout { get; private set; }
    
    //force overwrite: whether to force overwrite of existing files.
    //when specified, this will overwrite existing files without prompting.
    //this is controlled by -F or --force-overwrite.
    public bool ForceOverwrite { get; private set; }
    
    //minify: whether to minify the output.
    //when -m is also specified, this does nothing.
    //when specified, this will minify the HTML
    //this is controlled by -M or --minify.
    public bool Minify { get; private set; }
    
    
    //note about saving pages:
    //when one page is downloaded, it is written to STDOUT.
    //when multiple pages are downloaded, they are written to files in the current directory.
    //when -o is specified for multiple pages, the argument is treated as a directory and pages are written there instead of the current directory.
    //when -o is specified for one page, the argument is treated as a file and the page is written there instead of STDOUT.
    //when a file conflict is detected, the user is prompted to overwrite the file.
    //when -F is specified, the user is not prompted and the file is overwritten.
    //when -f is specified, the output is always sent to STDOUT, even if -o is specified.


    public void ProcessCommandLineArguments(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("fatal: expected an argument");
            Environment.Exit(1);
        }
        
        List<string> pages = new();
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith('-'))
            {
                //long flag
                if (args[i][1].Equals('-'))
                {
                    //switch-case for all long flags
                    switch (args[i][2..])
                    {
                        case "recursive":
                            Recursion = int.Parse(args[++i]);
                            break;
                        case "search":
                            //this is complicated so I won't implement it yet
                            break;
                        case "help":
                            Help = true;
                            break;
                        case "database":
                            TargetDatabase = args[++i];
                            break;
                        case "wiki":
                            TargetWiki = args[++i];
                            break;
                        case "output":
                            Output = args[++i];
                            break;
                        case "markdown":
                            Markdown = true;
                            break;
                        case "force-stdout":
                            ForceStdout = true;
                            break;
                        case "force-overwrite":
                            ForceOverwrite = true;
                            break;
                        case "minify":
                            Minify = true;
                            break;
                        case "index":
                            Index = true;
                            break;
                        case "verbose":
                            Verbose = true;
                            break;
                    }
                }

                //short flag
                else
                {
                    //switch-case for all short flags
                    foreach (char a in args[i])
                    {
                        switch (a)
                        {
                            case 'r': //recursive
                                Recursion = int.Parse(args[++i]);
                                break;
                            case 's': //search
                                //this is complicated so I won't implement it yet
                                break;
                            case 'h': //help
                                Help = true;
                                break;
                            case 'D': //database
                                TargetDatabase = args[++i];
                                break;
                            case 'W':
                                TargetWiki = args[++i];
                                break;
                            case 'o':
                                Output = args[++i];
                                break;
                            case 'm':
                                Markdown = true;
                                break;
                            case 'f':
                                ForceStdout = true;
                                break;
                            case 'F':
                                ForceOverwrite = true;
                                break;
                            case 'M':
                                Minify = true;
                                break;
                            case 'i':
                                Index = true;
                                break;
                            case 'v':
                                Verbose = true;
                                break;
                            default:
                                throw new ArgumentException($"Unknown argument: {a}");
                        }
                    }
                }
            }
            else
            {
                //position args
                pages.Add(args[i]);
            }
        }
        TargetPages = pages.ToArray();
    }
}