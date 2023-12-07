using Microsoft.AspNetCore.Mvc;

namespace BuyStuff.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
