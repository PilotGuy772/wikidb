using SharedLibrary.Application;
using SharedLibrary.Configuration;
using WikiServe;

namespace WikiDB;

internal static class Program
{
    public static int Main(string[] args)
    {
        ILocalConfig localConfig = new LocalConfig();
        localConfig.ProcessCommandLineArguments(args);
        return new ApplicationHost(Protocol.Serve, localConfig, GlobalConfig.ReadFromConfigFile()).Run(args);
    }
}