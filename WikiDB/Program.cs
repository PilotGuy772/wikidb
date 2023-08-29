using SharedLibrary.Configuration;

namespace WikiDB;

internal static class Program
{
    public static int Main(string[] args)
    {
        var localConfig = new WikiDBLocalConfig();
        localConfig.ProcessCommandLineArguments(args);
        return new ApplicationHost(localConfig, GlobalConfig.ReadFromConfigFile(localConfig.TargetDatabase, localConfig.TargetWiki)).Run(args);
    }
}