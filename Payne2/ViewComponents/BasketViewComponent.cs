using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payne.DAL.Context;
using Payne.ViewModels.Basket;
using Newtonsoft.Json;

namespace Payne.ViewComponents;

public class BasketViewComponent : ViewComponent
{
    AppDbContext _context;

    public BasketViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var json = Request.Cookies["basket"];
        List<CookieItemVm> cookies = new List<CookieItemVm>();

        if (json != null)
        {
            cookies = JsonConvert.DeserializeObject<List<CookieItemVm>>(json);
        }

        List<CartVm> cart = new List<CartVm>();
        List<CookieItemVm> deleteItem = new List<CookieItemVm>();

        if (cookies.Count > 0)
        {
            cookies.ForEach(c =>
            {
                var product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(p => p.Id == c.Id);

                if (product == null)
                {
                    deleteItem.Add(c);
                }
                else
                {
                    cart.Add(new CartVm()
                    {
                        Id = c.Id,
                        Price = product.Price,
                        Name = product.Name,
                        Count = c.Count,
                        ImgUrl = product.ProductImages.FirstOrDefault(p => p.Primary).ImgUrl
                    });
                }
            });
            if (deleteItem.Count > 0)
            {
                deleteItem.ForEach(d => { cookies.Remove(d); });
                HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookies));
            }
        }

        return View(cart);
    }
}