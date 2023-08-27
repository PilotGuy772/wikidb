using SharedLibrary.Application;
using SharedLibrary.Configuration;

namespace WikiServe;

internal static class Program
{
    public static int Main(string[] args)
    {
        /*ILocalConfig localConfig = new LocalConfig();
        localConfig.ProcessCommandLineArguments(args);
        return new ApplicationHost(Protocol.Serve, localConfig, GlobalConfig.ReadFromConfigFile(null, null)).Run(args);*/
        return 1;
    }
}