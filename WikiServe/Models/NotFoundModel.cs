namespace WikiServe.Models;


/// <summary>
/// model for the 404 page
/// </summary>
public class NotFoundModel
{
    public Uri RequestedUri;
    public int StatusCode;
    public string? TargetWiki;
    public string? TargetPage;

    public NotFoundModel(string uri, int code)
    {
        RequestedUri = new Uri(uri);
        StatusCode = code;
        string[] path = RequestedUri.AbsolutePath.Split('/');
        
        if (path.Length <= 1) return;
        TargetWiki = path[1];
        if (path.Length > 2)
        {
            TargetPage = path[2];
        }
    }
}