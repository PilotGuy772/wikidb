using SharedLibrary.Configuration;

namespace WikiDB;

public class LocalConfig : ILocalConfig
{
    //all command line arguments are represented here as public fields.
    
    
    // MODES //
    
    //list mode: list all databases as well as a summary of each database.
    //this is the default mode. If other behavior-changing command line arguments are specified, this argument is ignored.
    //This is controlled by -l or --list. If this argument is specified, the program will list all databases and then exit.
    //When -D is used to specify a database and this argument is specified, the program will list all wikis in the specified database and then exit.
    //When -W and -D are used together, the program will list all pages in the specified wiki and then exit.
    //When -W is used alone, the program will list all pages that are members of that wiki across all databases


    //long list mode: list all databases, all wikis in each database, and all pages in each wiki.
    //This is controlled by -L or --long-list. If this argument is specified, the program will list all databases and all wikis in each database and then exit.
    public bool LongList { get; private set; }
    
    //page list mode: list all pages and all wikis in a database.
    //this also lists some specific details of each page.
    //This is controlled by -P or --page-list. If this argument is specified, the program will list all pages in the specified database and then exit.
    public bool PageList { get; private set; }
    
    //page info mode: list all information about a page.
    //This is controlled by -i or --page-info. If this argument is specified, the program will list all information about the specified page and then exit.
    //Arguments -D and -W should be specified, otherwise the program will use all defaults.
    public bool PageInfo { get; private set; }
    
    //page delete mode: delete a page.
    //This is controlled by -d or --page-delete. If this argument is specified, the program will delete the specified page and then exit.
    //Arguments -D and -W should be specified, otherwise the program will use all defaults.
    public bool PageDelete { get; private set; }
    
    //wiki delete mode: delete a wiki.
    //This is controlled by -w or --wiki-delete. If this argument is specified, the program will delete the specified wiki and then exit.
    //In this case, -W is not necessary, as the wiki to delete is specified by the argument. When -D is used, the program will delete the specified wiki from the specified database.
    //When -D is omitted, the program will delete the specified wiki from all databases.
    public bool WikiDelete { get; private set; }
    
    //database delete mode: delete a database.
    //This is controlled by --database-delete. If this argument is specified, the program will delete the specified database and then exit.
    //In this case, -D is not necessary, as the database to delete is specified by the argument.
    public bool DatabaseDelete { get; private set; }
    
    
    // OPTIONS //
    
    //database: the database to use.
    //This is controlled by -D or --database. If this argument is specified, the program will use the specified database. If this argument is not specified, the program will use the default database.
    public string? TargetDatabase { get; private set; }
    
    //wiki: the wiki to use.
    //This is controlled by -W or --wiki. If this argument is specified, the program will use the specified wiki. If this argument is not specified, the program will use the default wiki.
    public string? TargetWiki { get; private set; }
    
    public string? TargetPage { get; private set; }
    

    public void ProcessCommandLineArguments(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            //if the argument is a flag, process it.
            if (!args[i].StartsWith('-')) continue;
            //long flag
            if (args[i][1].Equals('-'))
            {
                //switch case from the flag
                switch (args[i][2..])
                {
                    case "list":
                        LongList = true;
                        break;
                    case "long-list":
                        LongList = true;
                        break;
                    case "page-list":
                        PageList = true;
                        break;
                    case "page-info":
                        PageInfo = true;
                        i++;
                        TargetPage = args[i];
                        break;
                    case "page-delete":
                        PageDelete = true;
                        i++;
                        TargetPage = args[i];
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
                    default:
                        throw new ArgumentException($"Unknown argument: {args[i]}");
                }
            }

            else
            {
                //switch case from the flag
                
                foreach(char c in args[i][2..])
                    switch (c)
                    {
                        case 'l':
                            LongList = true;
                            break;
                        case 'L':
                            LongList = true;
                            break;
                        case 'P':
                            PageList = true;
                            break;
                        case 'i':
                            PageInfo = true;
                            i++;
                            TargetPage = args[i];
                            break;
                        case 'd':
                            PageDelete = true;
                            i++;
                            TargetPage = args[i];
                            break;
                        case 'w':
                            WikiDelete = true;
                            i++;
                            TargetWiki = args[i];   
                            break;
                        case 'D':
                            i++;
                            TargetDatabase = args[i];
                            break;
                        case 'W':
                            i++;
                            TargetWiki = args[i];
                            break;
                        default:
                            throw new ArgumentException($"Unknown argument: {c}");
                    }
            }
        }
    }
}