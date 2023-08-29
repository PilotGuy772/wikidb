using Microsoft.AspNetCore.Mvc;
using WikiServe.Models;

namespace WikiServe.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View(new IndexModel());
}