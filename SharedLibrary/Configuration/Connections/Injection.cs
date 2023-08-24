namespace SharedLibrary.Configuration.Connections;



/// <summary>
/// Represents an order to inject HTML into a page.
/// </summary>
public struct Injection
{
    public string Html; // stores the HTML to inject into the page.
    public string DestinationXpath; // The XPath statement representing the element to inject the HTML into.
    public bool InjectBefore; // Whether or not to inject the HTML before the element. If false, injects after the element.

    public Injection(string destination, string pathToHtml, bool injectBefore = true)
    {
        DestinationXpath = destination;
        Html = File.Exists(pathToHtml) ? File.ReadAllText(pathToHtml) : throw new FileNotFoundException("Could not find the HTML file referenced by injection order at path: " + pathToHtml);
    }
}