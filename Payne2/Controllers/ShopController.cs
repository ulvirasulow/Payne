using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payne.DAL.Context;
using Payne.Models;

namespace Payne.Controllers;

public class ShopController : Controller
{
    AppDbContext _context;

    public ShopController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Filter(string search)
    {
        return ViewComponent("Shop", search);
    }
    
}