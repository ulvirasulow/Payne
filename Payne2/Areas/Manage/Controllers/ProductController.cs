using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payne.Areas.Manage.ViewModels.Product;
using Payne.DAL.Context;
using Payne.Helpers.Extension;
using Payne.Models;

namespace Payne.Areas.Manage.Controllers;

[Area("Manage")]
[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(c => c.TagProducts)
            .ThenInclude(p => p.Tag)
            .Include(p => p.ProductImages)
            .ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Tags = await _context.Tags.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductVm vm)
    {
        ViewBag.Tags = await _context.Tags.ToListAsync();
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        Product product = new Product()
        {
            Name = vm.Name,
            Price = vm.Price,
            Description = vm.Description,
            Dimension = vm.Dimension,
            SKU = vm.SKU,
            Weight = vm.Weight,
            ProductImages = new List<ProductImages>()
        };

        if (vm.TagIds != null)
        {
            foreach (var tagId in vm.TagIds)
            {
                if (!(await _context.Tags.AnyAsync(t => t.Id == tagId)))
                {
                    ModelState.AddModelError("TagIds", $"{tagId} id-li tag yoxdur");
                    return View(vm);
                }

                _context.TagProducts.Add(new TagProduct()
                {
                    TagId = tagId,
                    Product = product
                });
            }
        }

        if (vm.Photo == null || !vm.Photo.ContentType.StartsWith("image/"))
        {
            ModelState.AddModelError("Photo", "Düzgün şəkil formatı daxil edin.");
            return View(vm);
        }

        if (vm.Photo.Length > 3000000)
        {
            ModelState.AddModelError("Photo", "Şəkil maksimum 3 MB ola bilər.");
            return View(vm);
        }

        string mainImgUrl = vm.Photo.Upload(_env.WebRootPath, "Upload/Product");
        if (string.IsNullOrEmpty(mainImgUrl))
        {
            ModelState.AddModelError("Photo", "Şəkil yüklənmədi.");
            return View(vm);
        }

        product.ProductImages.Add(new()
        {
            Primary = true,
            ImgUrl = mainImgUrl
        });

        if (vm.Images != null)
        {
            foreach (var item in vm.Images)
            {
                if (!item.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("Images", $"{item.FileName} düzgün şəkil formatında deyil.");
                    continue;
                }

                if (item.Length > 3000000)
                {
                    ModelState.AddModelError("Images", "Şəkil maksimum 3 MB ola bilər.");
                    continue;
                }

                string imgUrl = item.Upload(_env.WebRootPath, "Upload/Product");
                if (!string.IsNullOrEmpty(imgUrl))
                {
                    product.ProductImages.Add(new()
                    {
                        Primary = false,
                        ImgUrl = imgUrl
                    });
                }
            }
        }

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null || !(_context.Products.Any(c => c.Id == id)))
        {
            return View("Error");
        }

        var product = await _context.Products
            .Include(p => p.ProductImages)
            .Include(c => c.TagProducts)
            .ThenInclude(c => c.Tag)
            .FirstOrDefaultAsync(p => p.Id == id);

        ViewBag.Tags = _context.Tags.ToList();

        UpdateProductVm updateProductVm = new UpdateProductVm()
        {
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Dimension = product.Dimension,
            Weight = product.Weight,
            SKU = product.SKU,
            TagIds = new List<int>(),
            productImages = new List<ProductImageVm>()
        };

        foreach (var item in product.TagProducts)
        {
            updateProductVm.TagIds.Add(item.TagId);
        }

        foreach (var item in product.ProductImages)
        {
            updateProductVm.productImages.Add(new()
            {
                Primary = item.Primary,
                ImgUrl = item.ImgUrl
            });
        }

        return View(updateProductVm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateProductVm vm)
    {
        ViewBag.Tags = await _context.Tags.ToListAsync();

        if (vm.Id == 0 || !await _context.Products.AnyAsync(c => c.Id == vm.Id))
        {
            return View("Error");
        }

        var oldProduct = await _context.Products
            .Include(p => p.TagProducts)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == vm.Id);

        if (oldProduct == null)
        {
            return View("Error");
        }

        if (vm.TagIds != null && vm.TagIds.Any())
        {
            var existingTags = await _context.Tags
                .Where(t => vm.TagIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToListAsync();

            var invalidTags = vm.TagIds.Except(existingTags).ToList();
            if (invalidTags.Any())
            {
                ModelState.AddModelError("TagIds", $"{string.Join(", ", invalidTags)} id-li Taglar mövcud deyil..");
                return View(vm);
            }
        }

        _context.TagProducts.RemoveRange(oldProduct.TagProducts);

        if (vm.TagIds != null && vm.TagIds.Any())
        {
            foreach (var tagId in vm.TagIds)
            {
                _context.TagProducts.Add(new TagProduct()
                {
                    ProductId = oldProduct.Id,
                    TagId = tagId
                });
            }
        }

        if (vm.Photo != null)
        {
            if (!vm.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Düzgün şəkil formatı seçin");
                return View(vm);
            }

            if (vm.Photo.Length > 3000000)
            {
                ModelState.AddModelError("Photo", "Maksimum 3 mb-lıq şəkil yükləyə bilərsiz.");
                return View(vm);
            }

            var primaryImage = oldProduct.ProductImages.FirstOrDefault(x => x.Primary);
            if (primaryImage != null)
            {
                FileExtension.DeleteFile(_env.WebRootPath, "Upload/Product", primaryImage.ImgUrl);
                _context.ProductImages.Remove(primaryImage);
            }

            oldProduct.ProductImages.Add(new ProductImages()
            {
                Primary = true,
                ImgUrl = vm.Photo.Upload(_env.WebRootPath, "Upload/Product")
            });
        }

        if (vm.ImagesUrls != null)
        {
            var removeImages = oldProduct.ProductImages
                .Where(x => !x.Primary && !vm.ImagesUrls.Contains(x.ImgUrl))
                .ToList();

            foreach (var img in removeImages)
            {
                FileExtension.DeleteFile(_env.WebRootPath, "Upload/Product", img.ImgUrl);
                _context.ProductImages.Remove(img);
            }
        }
        else
        {
            var nonPrimaryImages = oldProduct.ProductImages.Where(x => !x.Primary).ToList();
            foreach (var img in nonPrimaryImages)
            {
                FileExtension.DeleteFile(_env.WebRootPath, "Upload/Product", img.ImgUrl);
                _context.ProductImages.Remove(img);
            }
        }

        if (vm.Images != null)
        {
            foreach (var img in vm.Images)
            {
                if (!img.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("Images", $"{img.FileName} düzgün şəkil formatında deyil!");
                    continue;
                }

                if (img.Length > 3000000)
                {
                    ModelState.AddModelError("Images", $"{img.FileName} 3 mb-dan böyükdür!");
                    continue;
                }

                oldProduct.ProductImages.Add(new ProductImages()
                {
                    Primary = false,
                    ImgUrl = img.Upload(_env.WebRootPath, "Upload/Product")
                });
            }
        }

        oldProduct.Name = vm.Name;
        oldProduct.Price = vm.Price;
        oldProduct.Description = vm.Description;
        oldProduct.Dimension = vm.Dimension;
        oldProduct.Weight = vm.Weight;
        oldProduct.SKU = vm.SKU;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id == null) return BadRequest();
        var product = _context.Products.FirstOrDefault(x => x.Id == id);
        if (product == null) return NotFound();
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}