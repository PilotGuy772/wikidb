using SharedLibrary.Data.Meta;

namespace WikiServe.Models;


/// <summary>
/// Model for the ViewWiki Razor view
/// </summary>
public class WikiModel
{
    public WikiReference WikiReference;

    public WikiModel(string targetWiki)
    {
        WikiReference = DatabaseMetadata.GetBasicMetadata(Program.GlobalConfig.DatabaseConnection).FirstOrDefault(x => x.Name.Equals(targetWiki));
    }
}