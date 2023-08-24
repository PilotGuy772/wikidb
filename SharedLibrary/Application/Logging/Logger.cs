namespace SharedLibrary.Application.Logging;


/// <summary>
/// Handles everything to do with logging to the console and to files. For now, there is no file logging.
/// </summary>
public static class Logger
{
    public static int LogFilter { get; set; } = 3;
    // 0 -> show nothing
    // 1 -> show only errors
    // 2 -> show only warnings and errors
    // 3 -> show all info, warnings, and errors
    
    public static void Log(string message, InfoTier infoTier)
    {
        //check if the requested infoTier is filtered out
        if ((int)infoTier < LogFilter) return;
        
        //log to console
        //note that exceptions and fatal errors are logged into STDERR instead of STDOUT.
        //This is handled by the exceptions handling logic in the main program.
        ConsoleColor consoleColor = infoTier switch
        {
            InfoTier.Info => ConsoleColor.White,
            InfoTier.Warning => ConsoleColor.Yellow,
            InfoTier.Error => ConsoleColor.Red,
            _ => throw new ArgumentOutOfRangeException(nameof(infoTier), infoTier, null)
        };
        ConsoleColor originalColor = Console.ForegroundColor;
        string prefix = infoTier switch
        {
            InfoTier.Info =>    "info:  ",
            InfoTier.Warning => "warn:  ",
            InfoTier.Error =>   "error: ",
            _ => throw new ArgumentOutOfRangeException(nameof(infoTier), infoTier, null)
        };
            

        Console.ForegroundColor = consoleColor;
        Console.WriteLine(prefix + message);
        Console.ForegroundColor = originalColor;
    }

    public static bool Prompt(string message, string defaultResponse = "no")
    {
        // prompt the user for a yes-or-no response about an issue
        string promptMsg = message + defaultResponse switch
        {
            "yes" or "y" => " [Y/n] ",
            "no" or "n" => " [y/N] ",
            _ => throw new ArgumentOutOfRangeException(nameof(defaultResponse), defaultResponse, null)
        };
        
        Console.Write(promptMsg);
        string input = Console.ReadLine() ?? "";
        return input.ToLower() switch
        {
            "yes" or "y" => true,
            "no" or "n" => false,
            _ => defaultResponse is "yes" or "y" //this just returns the default response.
        };
    }
}