using Microsoft.AspNetCore.Mvc;

namespace Payne.Controllers;

public class AccountController : Controller
{
    public IActionResult AccessDenied()
    {
        return View("NotFound");
    }
}