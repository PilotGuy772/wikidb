using System.Xml.XPath;
using SharedLibrary.Configuration;
using SharedLibrary.Configuration.Connections;

namespace WikiDB;

/// <summary>
/// Stores data for running the application and managing the application lifecycle. This class cannot be inherited.
/// </summary>
public class ApplicationHost
{
    public WikiDBLocalConfig LocalConfig { get; private set; }
    
    public GlobalConfig GlobalConfig { get; private set; }


    public ApplicationHost(WikiDBLocalConfig localConfig, GlobalConfig globalConfig)
    {
        LocalConfig = localConfig;
        GlobalConfig = globalConfig;
    }


    private static void BuildDatabase(DatabaseConnection connection)
    {
        //this is actually super simple. It just involves including a database.xml file and a pages/ directory.
        //first, create the pages/ directory. This will fill in any missing files along the way.
        string path = connection.Path.Contains('~')
            ? connection.Path.Replace("~", Environment.GetEnvironmentVariable("HOME"))
            : connection.Path;
        Directory.CreateDirectory(Path.Combine(path, "pages/"));
        
        //then, copy the default metadata file to the database.xml file. This is a hardcoded constant.
        const string defaultDatabaseMetadata = """<?xml version="1.0" encoding="UTF-8"?> <database> <name>{}</name> <wikis/> <pageCounter>0</pageCounter> </database>""";
        string save = defaultDatabaseMetadata.Replace("{}", connection.Name);
        File.WriteAllText(Path.Combine(path, "database.xml"), save);
        
        //all done! WikiGet and WikiDB should now be able to interface with this database.
    }
    
    
    public int Run(string[] args)
    {
        //this method controls the application lifecycle. For the most part, however, it just defers control right 
        //back to the appropriate project to handle the requested operation.
        
        //it also deals with errors and exceptions.
        try
        {

            //do different things based on configuration
            //in case we need to build, copy the default metadata file to the specified database
            if (LocalConfig.Build)
            {
                BuildDatabase(GlobalConfig.DatabaseConnection);
                return 0;
            }

            //in case we are doing a search, go to the Search namespace
            if (LocalConfig.ListMode)
            {
                Search.List.ListMode(LocalConfig);
                return 0;
            }
            
            //in case we are manipulating the database (removing pages or wikis), go to the Transform namespace

        }
        catch (Exception e)
        {
            //switch-case to handle possible exceptions in the solution and present to the user why they might be happening.
            //this is a very basic implementation and will be improved in the future.
            //the message embedded in the exception is printed first with the 'fatal: ' prefix
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("fatal: " + e.Message);
            Console.ForegroundColor = original;
            
            switch (e)
            {
                /*case InvalidDataException:
                    Console.Error.WriteLine("hint: a configuration or metadata file is malformed. You will have to fix this manually.\nhint: for help, this project's GitHub wiki may have examples.");
                    break;*/
                /*case ArgumentException:
                    break;*/
                case InvalidOperationException:
                    Console.Error.WriteLine("hint: the database may not be initialized. Run wikidb -bD <database> to initialize the database.");
                    break;
                case XPathException:
                    Console.Error.WriteLine("hint: verify the correctness of your XPath queries in the config file.");
                    break;
                /*case FileNotFoundException:
                    Console.Error.WriteLine("hint: copy or write a configuration file to ~/.config/wikidb/config.xml or /etc/wikidb/config.xml. Examples may be found on this project's GitHub wiki.");
                    break;*/
                case HttpRequestException:
                    Console.Error.WriteLine("hint: there was a problem with your local network or with the remote server. Ensure that this process has access to the internet.");
                    break;
                default:
                    Console.Error.WriteLine("hint: an unknown error occured during execution. The stack trace is shown below.");
                    Console.Error.WriteLine(e.StackTrace);
                    break;
            }
            
            return 1;
        }
        return 0;
    }
}