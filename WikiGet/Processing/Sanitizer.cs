using System.Text.RegularExpressions;
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
        
        
        //reroute <a> tags to point to the local page, even if the local page does not exist yet.
        //this is so that the user can download the page and then view it locally.
         /* After getting all <a> tags, this is the
          * PROCESS (for each <a> tag):
          * 1. Get the href attribute.
          * 2. Verify that it is a relative link
          * 3. Obtain the full path of the link by prepending the base URL.
          * 4. Use a regex to isolate the title of the page from the URL given the format of the URL.
          * 5. Generate the relative local path of the page (relative to document root when using WikiServe; it should be relative to the root of the database).
          * 6. Set the href of the <a> tag to this path.
          */
        HtmlNodeCollection aTags = doc.DocumentNode.SelectNodes("//a");
        string baseUrl = connection.BaseUrl;
        //remove all <a> tags that do not have an href attribute that starts with '/' or the base URL
        if (aTags != null)
        {
            foreach (HtmlNode aTag in aTags)
            {
                HtmlAttribute href = aTag.Attributes["href"];
                if (href != null)
                {
                    string hrefValue = href.Value;
                    if (!hrefValue.StartsWith("/") && !hrefValue.StartsWith(baseUrl)) continue;
                    //we have a relative link
                    //get the full path
                    string fullPath = hrefValue.StartsWith("/") ? baseUrl + hrefValue : hrefValue;
                    Uri uri = new(fullPath);
                    fullPath = uri.Scheme + "://" + uri.Host + uri.AbsolutePath; //remove query string and fragment
                    //regex to isolate the title of the page from the URL given the format of the URL
                    string pattern = Regex.Escape(connection.Url.Replace("{}", "")) + @"([^/?]+)";
                    //apply regex
                    Match match = Regex.Match(fullPath, pattern);
                    //check if it's a success
                    if (match.Success)
                    {
                        string title = match.Groups[1].Value;
                        //generate the relative local path of the page
                        string relativePath = Path.Combine(connection.Name, title) + uri.Fragment;
                        //update the <a> tag to match
                        href.Value = relativePath;
                    }
                    else
                    {
                        //throw an exception i guess?
                        /*throw new Exception("The URL " + fullPath + " does not match the format of the URL " + connection.Url + " ... somehow?");*/
                        //okay nvm just notify
                        /*Console.Error.WriteLine("warning: the URL " + fullPath + " does not match the format of the URL " + connection.Url + " ... somehow?");*/
                        //okay nvm actually just do nothing
                    }
                }
            }
        }
        
        
        //return the sanitized page
        //this is almost a copy of the argument page except for the modified HTML content.
        return new Page(page.Children, page.Name, doc.DocumentNode.InnerHtml, page.Parent, page.Wiki, page.Url, page.Path);

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
