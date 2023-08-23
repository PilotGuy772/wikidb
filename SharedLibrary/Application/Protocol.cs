namespace SharedLibrary.Application;

/// <summary>
/// Which application protocol to use.
/// </summary>
public enum Protocol
{
    Download, // for WikiGet
    Index, // for WikiDB
    Serve // for WikiServe
}