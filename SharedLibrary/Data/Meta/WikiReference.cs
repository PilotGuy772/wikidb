namespace SharedLibrary.Data.Meta;

//only stores the information about each wiki that can be found in the database meta file.
public struct WikiReference
{
    public string Name;
    public string Database;
    public string[] Pages;

    public WikiReference(string wikiName, string[] pages, string databaseName)
    {
        Name = wikiName;
        Pages = pages;
        Database = databaseName;
    }
}