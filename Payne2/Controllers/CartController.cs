using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Payne.DAL.Context;
using Payne.Helpers.Extension;
using Payne.ViewModels.Basket;

namespace Payne.Controllers;

public class CartController : Controller
{
    AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
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
                Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookies));
            }
        }

        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddBasket([FromBody] int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        List<CookieItemVm> cookiesList;

        var basket = Request.Cookies["basket"];

        if (basket != null)
        {
            cookiesList = JsonConvert.DeserializeObject<List<CookieItemVm>>(basket);
            var existProduct = cookiesList.FirstOrDefault(x => x.Id == id);

            if (existProduct != null)
            {
                existProduct.Count++;
            }
            else
            {
                cookiesList.Add(new CookieItemVm()
                {
                    Id = id,
                    Count = 1
                });
            }
        }
        else
        {
            cookiesList = new List<CookieItemVm>();
            cookiesList.Add(new CookieItemVm()
            {
                Id = id,
                Count = 1
            });
        }

        Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookiesList));

        return Ok();
    }

    public IActionResult GetBasket()
    {
        return Content(Request.Cookies["basket"]);
    }

    public IActionResult Refresh()
    {
        return ViewComponent("Basket");
    }

    public IActionResult GetBasketCount()
    {
        List<CookieItemVm> cookies = String.IsNullOrEmpty(Request.Cookies["basket"])
            ? new List<CookieItemVm>()
            : JsonConvert.DeserializeObject<List<CookieItemVm>>(Request.Cookies["basket"]);

        int count = cookies.Count == 0 ? 0 : cookies.Sum(x => x.Count);

        return Ok(count);
    }
}