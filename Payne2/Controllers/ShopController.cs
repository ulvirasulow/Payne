using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payne.DAL.Context;

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
        var products = _context.Products.Include(x => x.ProductImages).ToList();

        return View(products);
    }
    
    
}