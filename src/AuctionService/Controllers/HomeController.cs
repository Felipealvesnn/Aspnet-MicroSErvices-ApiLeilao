using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
