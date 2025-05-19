using Anyar.DAL;
using Anyar.Models;
using Anyar.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anyar.Areas.Admin.Controllers
{
    [Area("admin")]
    public class PositionController : Controller
    {
        private readonly AppDbContext _context;

        public PositionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<GetPositionVM> getPositionVMs = await _context.Positions.Select(p =>
                new GetPositionVM
                {
                    Name = p.Name,
                    Id = p.Id,
                }
            ).ToListAsync();
            return View(getPositionVMs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(GetPositionVM positionVM)
        {
            if (!ModelState.IsValid) return View();

            bool result = await _context.Positions.AnyAsync(p => p.Name == positionVM.Name);
            if (result)
            {
                ModelState.AddModelError(nameof(positionVM.Name), $"{positionVM.Name} is already exist");
            }

            Position position = new()
            {
                Name = positionVM.Name,
                Id = positionVM.Id,
            };

            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Position? position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);

            if (position is null) return NotFound();

            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Position? position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);

            if (position is null) return NotFound();

            UpdatePositionVM positionVM = new UpdatePositionVM()
            {
                Name = position.Name
            };
            return View(positionVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdatePositionVM positionVM)
        {
            if (!ModelState.IsValid) return View();

            bool result = await _context.Positions.AnyAsync(p => p.Name == positionVM.Name && p.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(Position.Name), $"position:{positionVM.Name} is already exists");
                return View();
            }

            Position? existed = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (existed.Name == positionVM.Name)
            {
                return RedirectToAction(nameof(Index));
            }

            existed.Name = positionVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
