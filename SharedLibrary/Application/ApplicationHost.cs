using SharedLibrary.Configuration;

namespace SharedLibrary.Application;

/// <summary>
/// Stores data for running the application and managing the application lifecycle. This class cannot be inherited. Note that the same class is used for both WikiGet and WikiDB.
/// </summary>
public class ApplicationHost
{
    /// <summary>
    /// The application protocol to use for the current instance.
    /// </summary>
    public Protocol Protocol { get; private set; }
    
    public ILocalConfig LocalConfig { get; private set; }
    
    public GlobalConfig GlobalConfig { get; private set; }


    public ApplicationHost(Protocol protocol, ILocalConfig localConfig, GlobalConfig globalConfig)
    {
        Protocol = protocol;
        LocalConfig = localConfig;
        GlobalConfig = globalConfig;
    }

    public int Run(string[] args)
    {
        throw new NotImplementedException();
    }
}