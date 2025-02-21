using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payne.DAL.Context;
using Payne.Helpers.Extension;
using Payne.Models;

namespace Payne.Areas.Manage.Controllers;

[Area("Manage")]
[Authorize(Roles = "Admin")]
public class SliderController : Controller
{
    AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public SliderController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        List<Slider> sliders = await _context.Sliders.ToListAsync();
        return View(sliders);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Slider slider)
    {
        if (!slider.File.ContentType.Contains("image"))
        {
            ModelState.AddModelError("File", "Duzdun sekil formati secin");
            return View();
        }

        if (slider.File.Length > 2097152)
        {
            ModelState.AddModelError("File", "Sekil max 2 Mb ola biler");
            return View();
        }

        slider.ImgUrl = slider.File.Upload(_env.WebRootPath, "Upload/Slider");

        _context.Sliders.Add(slider);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Update(int? id)
    {
        if (id == null) return BadRequest();

        var slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
        if (slider == null) return NotFound();

        return View(slider);
    }

    [HttpPost]
    public IActionResult Update(int? id, Slider slider)
    {
        if (id == null || slider == null) return BadRequest();

        var existSlider = _context.Sliders.FirstOrDefault(s => s.Id == id);
        if (existSlider == null) return NotFound();

        if (slider.File != null)
        {
            if (!slider.File.ContentType.Contains("image"))
            {
                ModelState.AddModelError("File", "Düzgün şəkil formatı seçin");
                return View(existSlider);
            }

            if (slider.File.Length > 2000000)
            {
                ModelState.AddModelError("File", "Şəkil maksimum 2 MB ola bilər");
                return View(existSlider);
            }

            FileExtension.DeleteFile(_env.WebRootPath, "Upload/Slider", existSlider.ImgUrl);
            existSlider.ImgUrl = slider.File.Upload(_env.WebRootPath, "Upload/Slider");
        }

        existSlider.Title = slider.Title;
        existSlider.SubTitle = slider.SubTitle;
        existSlider.Description = slider.Description;

        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public ActionResult Delete(int? id)
    {
        var slider = _context.Sliders.FirstOrDefault(c => c.Id == id);
        if (slider == null)
        {
            return NotFound();
        }

        FileExtension.DeleteFile(_env.WebRootPath, "Upload/Slider", slider.ImgUrl);
        _context.Sliders.Remove(slider);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}