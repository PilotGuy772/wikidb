using System.Xml.XPath;
using HtmlAgilityPack;
using SharedLibrary.Configuration.Connections;
using SharedLibrary.Data.Content;

namespace WikiGet.Processing;


/// <summary>
/// Sanitizes a downloaded page according to rules defined in configs.
/// </summary>
public class Sanitizer
{
    public static Page Sanitize(Page page, WikiConnection connection)
    {
        /*
         * PROCESS:
         * There must be a clearly defined order of operations so users can appropriately define and understand how they want sanitization to go.
         * 1. First, remove all removals.
         * 2. Second, inject all injections.
         * 3. Download all images IF REQUESTED and update <img> tags to point to the local image.
         */
        
        
        //we are using HtmlAgilityPack to parse the HTML
        HtmlDocument doc = new();
        doc.LoadHtml(page.Content);

        
        //removals
        foreach (Removal remove in connection.Removals)
        {
            switch (remove.Type)
            {
                case RemovalType.Xpath:
                    //Console.Error.WriteLine("info: trying to remove XPath: " + remove.Value);
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(remove.Value);
                    if (nodes != null)
                    {
                        foreach (HtmlNode node in nodes)
                        {
                            node.Remove();
                        }
                    }
                    break;
                case RemovalType.Id:
                    doc.GetElementbyId(remove.Value)?.Remove();
                    break;
                case RemovalType.TagType:
                    doc.DocumentNode.SelectNodes("//" + remove.Value)?.ToList().ForEach(x => x.Remove());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(connection));
            }
        }
        
        //injections
        foreach (Injection inject in connection.Injections)
        {
            if (inject.InjectBefore)
            {
                foreach (HtmlNode selectNode in doc.DocumentNode.SelectNodes(inject.DestinationXpath)  ?? throw new XPathException("The XPath expression " + inject.DestinationXpath + " did not match any nodes in the document.", new NullReferenceException()))
                {
                    selectNode.PrependChild(HtmlNode.CreateNode(ProcessContentInjections(inject.Html, page.Url)));
                }
            }
            else
            {
                foreach (HtmlNode selectNode in doc.DocumentNode.SelectNodes(inject.DestinationXpath) ?? throw new XPathException("The XPath expression " + inject.DestinationXpath + " did not match any nodes in the document.", new NullReferenceException()))
                {
                    selectNode.AppendChild(HtmlNode.CreateNode(ProcessContentInjections(inject.Html, page.Url)));
                }
            }
        }
        
        //download images
        //not implemented yet
        
        //return the sanitized page
        //this is almost a copy of the argument page except for the modified HTML content.
        return new Page(page.Children, page.Title, doc.DocumentNode.InnerHtml, page.Parent, page.Wiki, page.Url, page.Path);

        string ProcessContentInjections(string html, string url)
        {
            return html.Replace("{unixtime}", (DateTimeOffset.Now - DateTimeOffset.UnixEpoch).ToString())
                .Replace("{year}", DateTime.Now.Year.ToString())
                .Replace("{month}", DateTime.Now.Month.ToString())
                .Replace("{day}", DateTime.Now.Day.ToString())
                .Replace("{hour}", DateTime.Now.Hour.ToString())
                .Replace("{hour12}", DateTime.Now.Hour.ToString())
                .Replace("{minute}", DateTime.Now.Minute.ToString())
                .Replace("{second}", DateTime.Now.Second.ToString())
                .Replace("{url}", url);
        }
    }
}
