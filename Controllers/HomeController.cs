using Microsoft.AspNetCore.Mvc;
using e_Project.Models;

namespace e_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            var recentProjects = _context.Projects
                 .Where(p => p.Status == ProjectStatus.Completed)
                 .OrderByDescending(p => p.CompletionTime)
                 .Take(5)
                 .ToList();

            return View(recentProjects);
        }
    }
}
