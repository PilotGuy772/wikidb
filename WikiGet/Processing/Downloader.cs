using SharedLibrary.Configuration.Connections;
using SharedLibrary.Data.Content;

namespace WikiGet.Processing;


/// <summary>
/// Handles downloading pages from the internet
/// </summary>
public static class Downloader
{
    public static PageCollection DownloadPages(IEnumerable<string> pages, WikiConnection connection)
    {
        using HttpClient client = new();
        List<Page> pageList = new();
        
        foreach (string page in pages)
        {
            string url = connection.Url.Replace("{}", page);
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().Result;

                string pageNew = page;
                string? parent = null;
                //we need to generate part of the path (everything except the database root) here because it will not be possible later on.
                string partOfPath = Path.Combine("pages/", connection.Name, page + ".html"); 
                if (page.Contains('/'))
                {
                    // oh no... this is a subpage
                    string[] split = page.Split('/');
                    parent = Path.Combine(split[..1]); //the name of the parent page is stored as the absolute name of the page (including references to the parents of the parent, if they exist) instead of just the title of the parent page. This allows future problems to be dealt with, such as attempting to download a child page when the parent has not already been downloaded.
                    pageNew = split.Last();
                }
                
                pageList.Add(new Page(Array.Empty<Page>(), pageNew, content, parent, connection.Name, url, partOfPath));
            }
            else
            {
                throw new HttpRequestException("Download failed with error code: " + response.StatusCode);
            }
        }

        return new PageCollection(pageList);

        /*List<Page> pageList = (from page in pages 
            let url = connection.Url.Replace("{}", page) 
            let content = client.GetStringAsync(url).Result 
            select new Page(Array.Empty<Page>(), page, content, null, connection.Name, url, null)).ToList();*/
    }
}