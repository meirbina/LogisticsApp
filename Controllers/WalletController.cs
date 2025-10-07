using Microsoft.AspNetCore.Mvc;

namespace SMS.Controllers;

public class WalletController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}