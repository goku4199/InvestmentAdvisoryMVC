using Microsoft.AspNetCore.Mvc;

namespace InvestmentAdvisory.Controllers
{
    public class OAuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
