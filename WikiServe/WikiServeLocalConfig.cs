namespace WikiServe;

public class WikiServeLocalConfig
{
    //controls options for the local webserver
    //this particular class will be a singleton
    private static WikiServeLocalConfig? _instance = null;
    
    // OPTIONS //
    
    // port: the port to listen on
    // the default is 80.
    // controlled by -p or --port.
    public int Port { get; private set; } = 8080;
    
    // hostname: the hostname to listen on
    // the default is localhost.
    // this is controlled by -h or --hostname.
    public string Hostname { get; private set; } = "localhost";
    
    // root: the root directory to serve files from
    // by default, this is the root directory of the specified database.
    public string Root { get; private set; } = "";
    
    // database: the database to serve files from
    // controlled by -D or --database.
    public string? TargetDatabase { get; private set; }


    private WikiServeLocalConfig() { }

    public static WikiServeLocalConfig CreateInstance()
    {
        if (_instance != null) throw new InvalidOperationException("Only one may exist at a time, buckaroo");
        _instance = new WikiServeLocalConfig();
        return _instance;
    }

    public void ProcessCommandLineArguments(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith('-'))
            {
                if(args[i][1].Equals('-'))
                {
                    switch (args[i][2..])
                    {
                        case "port":
                            i++;
                            Port = int.Parse(args[i]);
                            break;
                        case "hostname":
                            i++;
                            Hostname = args[i];
                            break;
                        case "root":
                            i++;
                            Root = args[i];
                            break;
                        case "database":
                            i++;
                            TargetDatabase = args[i];
                            break;
                    }
                }
                else
                {
                    switch (args[i][1])
                    {
                        case 'p':
                            i++;
                            Port = int.Parse(args[i]);
                            break;
                        case 'h':
                            i++;
                            Hostname = args[i];
                            break;
                        case 'r':
                            i++;
                            Root = args[i];
                            break;
                        case 'D':
                            i++;
                            TargetDatabase = args[i];
                            break;
                    }
                }
            }
        }
    }
}