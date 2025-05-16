using Anyar.DAL;
using Anyar.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Anyar.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HomeVM vm = new HomeVM()
            {
                Employees = _context.Employees.ToList(),
                Positions = _context.Positions.ToList(),
            };

            return View(vm);
        }
    }
}
