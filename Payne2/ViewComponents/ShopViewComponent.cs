using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payne.DAL.Context;
using Payne.Models;

namespace Payne.ViewComponents;

public class ShopViewComponent : ViewComponent
{
    AppDbContext _context;

    public ShopViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(string searchTerm)
    {
        List<Product> products;
        if (searchTerm != null)
        {
            products = await _context.Products.Include(x => x.ProductImages)
                .Where(x => x.Name.ToLower().Contains(searchTerm.ToLower())).ToListAsync();
        }
        else
        {
            products = await _context.Products.Include(x => x.ProductImages).ToListAsync();
        }

        return View(products);
    }
}