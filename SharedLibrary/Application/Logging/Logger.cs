namespace SharedLibrary.Application.Logging;


/// <summary>
/// Handles everything to do with logging to the console and to files. For now, there is no file logging.
/// </summary>
public static class Logger
{
    public static int LogFilter { get; set; }
    // 0 -> show nothing
    // 1 -> show only errors
    // 2 -> show only warnings and errors
    // 3 -> show all info, warnings, and errors
    
    public static void Log(string message, Tier tier)
    {
        //check if the requested tier is filtered out
        if ((int)tier < LogFilter) return;
        
        //log to console
        //note that exceptions and fatal errors are logged into STDERR instead of STDOUT.
        //This is handled by the exceptions handling logic in the main program.
        ConsoleColor consoleColor = tier switch
        {
            Tier.Info => ConsoleColor.White,
            Tier.Warning => ConsoleColor.Yellow,
            Tier.Error => ConsoleColor.Red,
            _ => throw new ArgumentOutOfRangeException(nameof(tier), tier, null)
        };
        ConsoleColor originalColor = Console.ForegroundColor;
        string prefix = tier switch
        {
            Tier.Info =>    "info:  ",
            Tier.Warning => "warn:  ",
            Tier.Error =>   "error: ",
            _ => throw new ArgumentOutOfRangeException(nameof(tier), tier, null)
        };
            

        Console.ForegroundColor = consoleColor;
        Console.WriteLine(prefix + message);
        Console.ForegroundColor = originalColor;
    }
}