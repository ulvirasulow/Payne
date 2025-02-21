/*using Microsoft.AspNetCore.Mvc;
using Payne.DAL.Context;

namespace Payne.ViewComponents;

public class ProductViewComponent : ViewComponent
{
    AppDbContext _context;

    public ProductViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(string key)
    {
        List<Product> products = new List<Product>();

        switch (key.ToLower())
        {
            case "latest":
                products = await _context.Products.OrderByDescending(x => x.Id).ToListAsync();
                break;
            case "bestseller":
                products = await _context.Products.OrderBy(x => x.Count).ToListAsync();
                break;
            case "featured":
                products = await _context.Products.OrderBy(x => x.Count).ToListAsync();
                break;
            default:
                break;
        }

        return View(products);
    }
}*/