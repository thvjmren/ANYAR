using Anyar.DAL;
using Anyar.Models;
using Anyar.ViewModels;
using Microsoft.AspNetCore.Mvc;
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

            string fileName = await employeeVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img", "team");

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

            employee.Image.DeleteFile(_env.WebRootPath, "assets", "img");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Employee? employee = await _context.Employees.Include(p => p.Position)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee is null) return NotFound();

            List<Position> positions = await _context.Positions.ToListAsync();

            UpdateEmployeeVM updateEmployeeVM = new()
            {
                Name = employee.Name,
                Image = employee.Image,
                Facebook = employee.Facebook,
                Instagram = employee.Instagram,
                X = employee.X,
                Linkedin = employee.Linkedin,
                PositionName = employee.Position.Name,
                Positions = positions
            };

            return View(updateEmployeeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateEmployeeVM employeeVM)
        {
            employeeVM.Positions = await _context.Positions.ToListAsync();

            if (!ModelState.IsValid) return View(employeeVM);

            if (employeeVM.Photo is not null)
            {
                if (!employeeVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateEmployeeVM.Photo), "only image");
                    return View(employeeVM);
                }
                if (!employeeVM.Photo.ValidateSize(FileSize.KB, 1000))
                {
                    ModelState.AddModelError(nameof(UpdateEmployeeVM.Photo), "file size must be less than 500 KB");
                    return View(employeeVM);
                }
            }

            Employee? employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employee is null) return NotFound();

            if (employeeVM.Photo is not null)
            {
                employee.Image.DeleteFile(_env.WebRootPath, "assets", "img", "team");
                employee.Image = await employeeVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img", "team");
            }

            employee.Name = employeeVM.Name;
            employee.Facebook = employeeVM.Facebook;
            employee.X = employeeVM.X;
            employee.Instagram = employeeVM.Instagram;
            employee.Linkedin = employeeVM.Linkedin;
            employee.PositionId = employeeVM.PositionId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
