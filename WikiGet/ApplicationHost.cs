using System.Xml.XPath;
using SharedLibrary.Application;
using SharedLibrary.Configuration;

namespace WikiGet;

/// <summary>
/// Stores data for running the application and managing the application lifecycle. This class cannot be inherited.
/// </summary>
public class ApplicationHost
{
    /// <summary>
    /// The application protocol to use for the current instance.
    /// </summary>
    public ILocalConfig LocalConfig { get; private set; }
    
    public GlobalConfig GlobalConfig { get; private set; }


    public ApplicationHost(ILocalConfig localConfig, GlobalConfig globalConfig)
    {
        LocalConfig = localConfig;
        GlobalConfig = globalConfig;
    }

    public int Run(string[] args)
    {
        //this method controls the application lifecycle. For the most part, however, it just defers control right 
        //back to the appropriate project to handle the requested operation.
        
        //it also deals with errors and exceptions.
        try
        {

            global::WikiGet.WikiGet.Start(LocalConfig, GlobalConfig);

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
                case InvalidDataException:
                    Console.Error.WriteLine("hint: a configuration or metadata file is malformed. You will have to fix this manually.\nhint: for help, this project's GitHub wiki may have examples.");
                    break;
                /*case ArgumentException:
                    break;*/
                case InvalidOperationException:
                    Console.Error.WriteLine("hint: run wikidb -bD <database> to initialize the database.");
                    break;
                case XPathException:
                    Console.Error.WriteLine("hint: verify the correctness of your XPath queries in the config file.");
                    break;
                case FileNotFoundException:
                    Console.Error.WriteLine("hint: copy or write a configuration file to ~/.config/wikidb/config.xml or /etc/wikidb/config.xml. Examples may be found on this project's GitHub wiki.");
                    break;
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