namespace SharedLibrary.Configuration.Connections;


/// <summary>
/// Manages a connection to a wiki on the internet. This class cannot be inherited.
/// </summary>
public class WikiConnection
{
    public string Name { get; private set; } // stores the name of the wiki. Note that this is not the URL of the wiki. It should be all lowercase and contain no spaces.
    public string Url { get; private set; } // stores the URL for the wiki. Note that it will be formatted as "https://wiki.example.com/wiki/{}?lang=en_us" where {} is the page name.
    public Injection[] Injections { get; private set; } // stores the injections for the wiki. Note that the injections will be injected in the order they are stored in this array.
    public Removal[] Removals { get; private set; } // stores the removals for the wiki. Note that the removals will be removed in the order they are stored in this array. Removals are competed before injections.
    public bool DownloadReferencedImages { get; private set; }
    
    public WikiConnection(string name, string url, Injection[] injections, Removal[] removals)
    {
        Name = name;
        Url = url;
        Injections = injections;
        Removals = removals;
    }

    public WikiConnection(string name, string url, bool downloadReferencedImage, IEnumerable<Injection> injections, IEnumerable<Removal> removals)
    {
        Name = name;
        Url = url;
        DownloadReferencedImages = downloadReferencedImage; // NOT IMPLEMENTED YET
        Injections = injections.ToArray();
        List<Removal> removalsList = 
            (from removal in removals 
                let names = removal.Value.Split(' ') 
                from id in names 
                select new Removal(removal.Type, id)).ToList();
        Removals = removalsList.ToArray();
    }
}