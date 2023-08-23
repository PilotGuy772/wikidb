namespace SharedLibrary.Configuration;


/// <summary>
/// Interface to represent a configuration class that is specific to the current instance of the application. Mostly stores application-specific flags and command-line arguments.
/// </summary>
public interface ILocalConfig
{
    public void ProcessCommandLineArguments(string[] args); // processes command-line arguments
    public string? TargetDatabase { get; } // stores the name of the database to use.
    public string? TargetWiki { get; } // stores the name of the wiki to use.
}