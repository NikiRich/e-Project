using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using e_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace e_Project.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult AddProject()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProject(Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(project);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            List<Project> projects = new List<Project>();

            if (!string.IsNullOrEmpty(query))
            {
                var matches = await _context.Projects.Where(p => p.Title.Contains(query)
                || p.Description.Contains(query) || p.Author.Contains(query)
                || p.ProgrammingLanguage.Contains(query))
                    .ToListAsync();

                projects = matches.Select(p => new
                {
                    Project = p,
                    Occurences = (p.Title.CountOccurrences(query) +
                    p.Description.CountOccurrences(query) + p.Author.CountOccurrences(query) +
                    p.ProgrammingLanguage.CountOccurrences(query))
                }).OrderByDescending(p => p.Occurences).Select(p => p.Project).ToList();
            }

            ViewBag.query = query;
            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project != null && (project.Status == ProjectStatus.Pending || project.Status == ProjectStatus.Completed))
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Project updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), "Home");
            }
            return View(project);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                TempData["Message"] = "Please select a JSON file to upload.";
                TempData["MessageType"] = "Error";
                return View();
            }

            try
            {
                using (var stream = jsonFile.OpenReadStream())
                {
                    var projects = await JsonSerializer.DeserializeAsync<List<Project>>(stream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (projects != null)
                    {
                        foreach (var project in projects)
                        {
                            _context.Projects.Add(project);
                        }
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = $"{projects.Count} projects have been successfully imported.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "The file could not be deserialized into the project list.";
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                TempData["ErrorMessage"] = "There was an error processing your JSON file: " + jsonEx.Message;
                return RedirectToAction(nameof(Import));
            }
            catch (Exception ex)
            {
                TempData["Message"] = "An unexpected error occurred: " + ex.Message;
                TempData["MessageType"] = "Error";
            }

            return View();
        }



    }
    public static class Extension
    {
        public static int CountOccurrences(this string target, string value)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(value))
            {
                return 0;
            }

            int count = 0;
            int i = 0;
            while ((i = target.IndexOf(value, i, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                i += value.Length;
                count++;
            }

            return count;
        }
    }

}
