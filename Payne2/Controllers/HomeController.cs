using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payne.DAL.Context;
using Payne.Models;
using Payne.ViewModels.Home;

namespace Payne.Controllers;

public class HomeController : Controller
{
    AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        Response.Cookies.Append("0f3c6a5d-9974-43e3-b3ea-fab0976bb41e", "f5f45334-11d2-4aa0-9d11-be649315a017",
            new CookieOptions()
            {
                MaxAge = TimeSpan.FromHours(3)
            });

        HttpContext.Session.SetString("ab107", "ey tepegoz");

        List<Product> products = await _context.Products.Include(p => p.ProductImages).ToListAsync();
        List<Slider> sliders = await _context.Sliders.ToListAsync();

        HomeVm model = new HomeVm
        {
            Products = products,
            Sliders = sliders
        };
        return View(model);
    }

    public async Task<IActionResult> Detail(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.TagProducts)
            .ThenInclude(p => p.Tag)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        var relatedProducts = await _context.Products
            .Include(x => x.ProductImages)
            .Where(x => x.Price == product.Price && x.Id != product.Id)
            .ToListAsync();

        var viewModel = new DetailVm
        {
            Product = product,
            ProductImages = product.ProductImages.ToList()
        };

        ViewBag.RelatedProducts = relatedProducts;

        return View(viewModel);
    }

    public IActionResult ContactUs()
    {
        return View();
    }

    public IActionResult WishList()
    {
        return View();
    }
}