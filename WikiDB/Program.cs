using SharedLibrary.Application;
using SharedLibrary.Configuration;

namespace WikiDB;

internal static class Program
{
    public static int Main(string[] args)
    {
        ILocalConfig localConfig = new LocalConfig();
        localConfig.ProcessCommandLineArguments(args);
        return new ApplicationHost(Protocol.Index, localConfig, GlobalConfig.ReadFromConfigFile(localConfig.TargetDatabase, localConfig.TargetWiki)).Run(args);
    }
}