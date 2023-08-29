using SharedLibrary.Configuration;

namespace WikiServe;

internal static class Program
{
    public static WikiServeLocalConfig LocalConfig;
    public static GlobalConfig GlobalConfig;
    
    public static int Main(string[] args)
    {
        //WikiServe uses a different program arrangement from the rest
        //It's an ASP.NET Core application, not a console app
        //initialize local config
        LocalConfig = WikiServeLocalConfig.CreateInstance();
        LocalConfig.ProcessCommandLineArguments(args);
        
        //initialize the global config
        GlobalConfig = GlobalConfig.ReadFromConfigFile(LocalConfig.TargetDatabase, null);
        
        
        IHost host = CreateHostBuilder(args).Build();

        host.Run();
        
        return 0;
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                //if the hostname and port is specified, use those
                webBuilder.UseUrls($"http://{LocalConfig.Hostname}:{LocalConfig.Port}");
                
                webBuilder.UseStartup<Startup>();
            });
}