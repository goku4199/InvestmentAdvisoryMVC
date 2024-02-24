using InvestmentAdvisory.Data;
using InvestmentAdvisory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestmentAdvisory.Controllers
{
    public class AdvisorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdvisorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Advisor advisor)
        {
            // Add your login logic here
            var advisorInDb = await _context.Advisors
                .FirstOrDefaultAsync(a => a.Email == advisor.Email && a.Password == advisor.Password);

            if (advisorInDb != null)
            {
                // Create a new session for the advisor
                HttpContext.Session.SetInt32("AdvisorId", advisorInDb.AdvisorId);
                return RedirectToAction(nameof(Index)); // Redirect to the index action after successful login
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(advisor);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Advisor advisor)
        {
            // Add your registration logic here
            if (ModelState.IsValid)
            {
                _context.Add(advisor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(advisor);
        }
    }
}
