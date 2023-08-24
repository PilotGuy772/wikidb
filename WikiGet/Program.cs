using SharedLibrary.Configuration;

namespace WikiGet;

internal static class Program
{
    public static int Main(string[] args)
    {
        ILocalConfig localConfig = new WikiGetLocalConfig();
        localConfig.ProcessCommandLineArguments(args);
        return new ApplicationHost(localConfig, GlobalConfig.ReadFromConfigFile()).Run(args);
    }
}