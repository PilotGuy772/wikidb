namespace SharedLibrary.Configuration.Connections;


/// <summary>
/// Manages a connection to a local database. This class cannot be inherited.
/// </summary>
public class DatabaseConnection
{
    public string Name { get; private set; } // stores the name of the database.
    public string Path { get; private set; } // stores the path to the root directory of the database.
    public int PageCounter { get; private set; }
    
    public DatabaseConnection(string name, string path, int pages = 0)
    {
        Name = name;
        Path = path;
        PageCounter = pages;
    }
}