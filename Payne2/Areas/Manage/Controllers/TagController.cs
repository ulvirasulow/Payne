using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payne.DAL.Context;
using Payne.Models;

namespace Payne.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]
    public class TagController : Controller
    {
        AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tags = await _context.Tags
                .Include(c => c.TagProducts)
                .ThenInclude(x => x.Product)
                .ToListAsync();
            return View(tags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tags tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            var tag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (tag == null) return NotFound();
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();
            var tag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Tags newTag)
        {
            var oldTag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == newTag.Id);
            if (oldTag == null) return NotFound();

            oldTag.Name = newTag.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}