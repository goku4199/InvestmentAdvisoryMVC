using InvestmentAdvisory.Data;
using InvestmentAdvisory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestmentAdvisory.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Client client)
        {
            // Add your login logic here
            var clientInDb = await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == client.Email && c.Password == client.Password);

            if (clientInDb != null)
            {
                // Create a new session for the client
                HttpContext.Session.SetInt32("ClientId", clientInDb.ClientId);
                return RedirectToAction(nameof(Index)); // Redirect to the index action after successful login
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(client);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Client client)
        {
            // Add your registration logic here
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(client);
        }
    }
}
