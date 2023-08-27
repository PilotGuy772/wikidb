using SharedLibrary.Configuration;

namespace WikiDB;

public class WikiDBLocalConfig : ILocalConfig
{
    //all command line arguments are represented here as public fields.
    
    
    /*
     * SEARCH MODES:
     * Search mode is invoked with -l or --list. By default, search mode ONLY outputs the names of the results, not any other info.
     * -> Default: when -l is used alone, a tree structure rendering of the registered databases, wikis, and pages is shown.
     * -> Database List Mode: When -ld is used together, this lists all registered databases.
     * -> Wiki List Mode: When -lw is used together, this lists all registered wikis.
     * -> Page List Mode: When -lp is used together, this lists all registered pages.
     * -> Info Mode: When -i is used together with any of the above modes, all accessible information about each row is also listed.
     * -> Column Mode: When -c <column-name> is used together with any of the above modes, ONLY the specified column is shown. This option may be used as many times as desired to produce multiple columns.
     * -> Specific List Mode: Lists only the content one tier below the lowest tiered argument. For example, if -D is used, only the wikis in the specified database are listed. If -W is used, only the pages in the specified wiki are listed.
     *      -> These are specified by -D and -W. When -D is used, the program will list only the wikis in the specified database. When -W is used, the program will list only the pages in the specified wiki.
     *      -> When the two are used together, the program will list only the pages in the specified wiki in the specified database.
     *
     * Specific List Mode is incompatible with traditional list modes (Wiki, Database, or Page List Mode).
     */
    
    // SEARCH MODES //
    
    //default: controlled by -l or --list.
    //all search modes require -l to be invoked. When any other search option (besides Info and Column) is used, this is overrided.
    public bool ListMode { get; private set; }
    
    //database mode: controlled by -d or --database-list-mode
    public bool DatabaseListMode { get; private set; }
    
    //wiki mode: controlled by -w or --wiki-list-mode
    public bool WikiListMode { get; private set; }
    
    //page mode: controlled by -p or --page-list-mode
    public bool PageListMode { get; private set; }
    
    //info mode: controlled by -i or --info
    //this is a modifier for any of the above modes. When this is used, the program will print all available information about each row.
    public bool InfoMode { get; private set; }
    
    //column mode: controlled by -c or --column
    //this is a modifier for any of the above modes. When this is used, the program will print ONLY the specified column(s).
    //if null, this is ignored.
    public List<string>? Columns { get; private set; }
    
    /*
     * COLUMN NAMES:
     * For databases, wikis, and pages: name
     * For databases: name, path, pageCounter
     * For wikis: name, url
     * For pages: name, url, wiki, database, path
     *
     * Column filters are stored by their string names in the Columns list.
     */
    
    //specific list modes are ahead. These override any other search mode options. See the summary above for how they interact with each other.
    
    //database specific list mode: the database to use for specific list mode
    //this is controlled by -D or --database. If this argument is specified, the program will use the specified database.
    public string? TargetDatabase { get; private set; }
    
    //wiki specific list mode: the wiki to use for specific list mode
    //this is controlled by -W or --wiki. If this argument is specified, the program will use the specified wiki.
    public string? TargetWiki { get; private set; }


    // DATABASE MANIPULATION //
    
    //build mode: if a database is listed in the config file but has not yet been initialized, this generates necessary files and directories
    //This is controlled by -b or --build. It should be used together with -D, otherwise the default database will be specified.
    public bool Build { get; private set; }
    
    
    //page delete mode: delete a page.
    //This is controlled by -d or --page-delete. If this argument is specified, the program will delete the specified page and then exit.
    //This overrides any search mode options.
    //Arguments -D and -W should be specified, otherwise the program will use all defaults.
    public bool PageDelete { get; private set; }
    
    //wiki delete mode: delete a wiki.
    //This is controlled by -w or --wiki-delete. If this argument is specified, the program will delete the specified wiki and then exit.
    //In this case, -W is not necessary, as the wiki to delete is specified by the argument. When -D is used, the program will delete the specified wiki from the specified database.
    //This overrides any search mode options.
    //When -D is omitted, the program will delete the specified wiki from all databases.
    public bool WikiDelete { get; private set; }
    
    //database delete mode: delete a database.
    //This is controlled by --database-delete. If this argument is specified, the program will delete the specified database and then exit.
    //This overrides any search mode options.
    //In this case, -D is not necessary, as the database to delete is specified by the argument.
    public bool DatabaseDelete { get; private set; }
    
    
    // OPTIONS //
    
    
    //verbose: whether or not to print verbose output.
    //This is controlled by -v or --verbose. If this argument is specified, the program will print verbose output.
    public bool Verbose { get; private set; }
    
    

    public void ProcessCommandLineArguments(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("fatal: expected an argument.");
            Environment.Exit(1);
        }
        
        for (var i = 0; i < args.Length; i++)
        {
            //if the argument is a flag, process it.
            if (!args[i].StartsWith('-'))
            {
                Console.WriteLine("fatal: this program does not use positional arguments.");
                Environment.Exit(1);
            }

            //long flag
            if (args[i][1].Equals('-'))
            {
                //switch case from the flag
                switch (args[i][2..])
                {
                    case "list":
                        ListMode = true;
                        break;
                    case "database-list-mode":
                        DatabaseListMode = true;
                        break;
                    case "wiki-list-mode":
                        WikiListMode = true;
                        break;
                    case "page-list-mode":
                        PageListMode = true;
                        i++;
                        break;
                    case "info":
                        InfoMode = true;
                        break;
                    case "column":
                        i++;
                        Columns ??= new List<string>();
                        Columns.Add(args[i]);
                        break;
                    case "database":
                        i++;
                        TargetDatabase = args[i];
                        break;
                    case "wiki":
                        i++;
                        TargetWiki = args[i];
                        break;
                    case "page-delete":
                        PageDelete = true;
                        i++;
                        break;
                    case "wiki-delete":
                        WikiDelete = true;
                        i++;
                        TargetWiki = args[i];
                        break;
                    case "database-delete":
                        DatabaseDelete = true;
                        i++;
                        TargetDatabase = args[i];
                        break;
                    case "verbose":
                        Verbose = true;
                        break;
                    case "build":
                        Build = true;
                        break;
                    default:
                        throw new ArgumentException($"Unknown argument: {args[i]}");
                }
            }

            else
            {
                //switch case from the flag
                
                foreach(char c in args[i][1..])
                    switch (c)
                    {
                        case 'l':
                            ListMode = true;
                            break;
                        case 'd':
                            DatabaseListMode = true;
                            break;
                        case 'w':
                            WikiListMode = true;
                            break;
                        case 'p':
                            PageListMode = true;
                            break;
                        case 'i':
                            InfoMode = true;
                            break;
                        case 'c':
                            i++;
                            Columns ??= new List<string>();
                            Columns.Add(args[i]);
                            break;
                        case 'D':
                            i++;
                            TargetDatabase = args[i];
                            break;
                        case 'W':
                            i++;
                            TargetWiki = args[i];
                            break;
                        case 'v':
                            Verbose = true;
                            break;
                        case 'b':
                            Build = true;
                            break;
                        default:
                            throw new ArgumentException($"Unknown argument: {c}");
                    }
            }
        }
    }
}