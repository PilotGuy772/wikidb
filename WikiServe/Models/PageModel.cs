using HtmlAgilityPack;
using SharedLibrary.Data.Content;

namespace WikiServe.Models;

/// <summary>
/// Essentially a wrapper for an HtmlDocument, but it also contains a Page object for metadata.
/// </summary>
public class PageModel
{
    public readonly HtmlDocument Content;
    public readonly Page Page;

    public PageModel(Page page)
    {
        Content = new HtmlDocument();
        Content.LoadHtml(page.Content);
        HtmlNodeCollection titleTags = Content.DocumentNode.SelectNodes("//title");
        if (titleTags != null)
        {
            foreach (HtmlNode titleTag in titleTags)
            {
                titleTag.Remove();
            }
        }
        
        Page = page;
    }
}