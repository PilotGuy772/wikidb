using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Application.Logging;
using SharedLibrary.Data.Content;
using SharedLibrary.Data.Meta;
using WikiServe.Models;

namespace WikiServe.Controllers;

public class PageController : Controller
{
    public IActionResult ViewPage(string wiki, string page)
    {
        /*
         * PROCESS:
         * 1. Retrieve the page from the database.
         * 2. Load it as an HtmlDocument.
         * 3. Render the page (send to razor view).
         * ezpz
         */


        try
        {
            /*return View(new PageModel(PageMetadata.GetPage(page, wiki, Program.GlobalConfig.DatabaseConnection)));*/
            return Content(PageMetadata.GetPage(page, wiki, Program.GlobalConfig.DatabaseConnection).Content,
                "text/html");
        }
        catch (Exception e)
        {
            Logger.Log("exception thrown during page fetch: " + e.Message, InfoTier.Error);
            return NotFound();
        }
    }

    public IActionResult ViewWiki(string wiki)
    {
        try
        {
            return View(new WikiModel(wiki));
        }
        catch (Exception e)
        {
            Logger.Log("exception thrown during wiki fetch: " + e.Message, InfoTier.Error);
            return NotFound();
        }
    }
}