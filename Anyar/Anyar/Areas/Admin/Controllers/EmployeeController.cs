using Anyar.DAL;
using Anyar.Models;
using Anyar.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Extensions;

namespace Anyar.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetEmployeeVM> employeeVMs = await _context.Employees.Select(e =>
            new GetEmployeeVM
            {
                Name = e.Name,
                Id = e.Id,
                Image = e.Image,
                Instagram = e.Instagram,
                Facebook = e.Facebook,
                Linkedin = e.Linkedin,
                X = e.X,
                PositionName = e.Position.Name
            }).ToListAsync();
            return View(employeeVMs);
        }

        public async Task<IActionResult> Create()
        {
            CreateEmployeeVM employeeVM = new()
            {
                Positions = await _context.Positions.ToListAsync()
            };
            return View(employeeVM);
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateEmployeeVM employeeVM)
        {
            employeeVM.Positions = await _context.Positions.ToListAsync();

            if (!ModelState.IsValid) return View(employeeVM);

            if (!employeeVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateEmployeeVM.Photo), "file type is incorrect");
                return View(employeeVM);
            }

            if (!employeeVM.Photo.ValidateSize(FileSize.MB, 1))
            {
                ModelState.AddModelError(nameof(CreateEmployeeVM.Photo), "file size should be less than 1MB");
                return View(employeeVM);
            }

            string fileName = await employeeVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img");

            Employee employee = new()
            {
                Name = employeeVM.Name,
                Image = fileName,
                Instagram = employeeVM.Instagram,
                X = employeeVM.X,
                Facebook = employeeVM.Facebook,
                Linkedin = employeeVM.Linkedin,
                PositionId = employeeVM.PositionId,
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Employee? employee = await _context.Employees.FirstOrDefaultAsync(p => p.Id == id);

            if (employee is null) return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
