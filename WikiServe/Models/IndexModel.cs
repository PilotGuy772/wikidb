using SharedLibrary.Configuration.Connections;
using SharedLibrary.Data.Meta;

namespace WikiServe.Models;

/// <summary>
/// Stores information necessary for Index.cshtml. This includes a DatabaseConnection and an array of WikiReference.
/// </summary>
public class IndexModel
{
    public DatabaseConnection DatabaseConnection;
    public IEnumerable<WikiReference> WikiReferences;

    public IndexModel()
    {
        //automatically fetches the active database and all WikiConnections
        DatabaseConnection = Program.GlobalConfig.DatabaseConnection;
        WikiReferences = DatabaseMetadata.GetBasicMetadata(DatabaseConnection);
    }
}