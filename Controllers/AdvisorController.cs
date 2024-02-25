using InvestmentAdvisory.Data;
using InvestmentAdvisory.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InvestmentAdvisory.Controllers
{
    public class AdvisorController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private Advisor advisor; // Declare advisor variable at the class level

        public AdvisorController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //JWT TOKEN BASED AUTHENTICATION
        [HttpPost]
        public async Task<IActionResult> Login(Advisor advisor)
        {
            var advisorInDb = await _context.Advisors
                .FirstOrDefaultAsync(a => a.Email == advisor.Email && a.Password == advisor.Password);

            if (advisorInDb != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Replace with your secret key
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                            new Claim(ClaimTypes.Name, advisorInDb.AdvisorId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { Token = tokenString });
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return Unauthorized();
        }

        [HttpGet]
        public IActionResult LoginWithGoogle(string returnUrl = "/Advisor/GoogleResponse")
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl }, GoogleDefaults.AuthenticationScheme);
        }

        //OAuth + JWT Token based Authentication
        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Succeeded != true)
            {
                return BadRequest("Google authentication failed");
            }

            var emailClaim = result.Principal.FindFirst(ClaimTypes.Email);
            if (emailClaim == null)
            {
                return BadRequest("Google authentication did not provide an email");
            }

            var email = emailClaim.Value;

            advisor = await _context.Advisors.FirstOrDefaultAsync(a => a.Email == email);
            if (advisor == null)
            {
                // Create a new advisor
                advisor = new Advisor
                {
                    Email = email,
                    // Set other properties as necessary
                    Name = "Default Name",
                    Specialty = "Default Specialty",
                    Password = "Default Password"
                };
                _context.Advisors.Add(advisor);
                await _context.SaveChangesAsync();
            }

            // Create a JWT for the advisor and return it in the same way you do in your existing Login action
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Replace with your secret key
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, advisor.AdvisorId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
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
