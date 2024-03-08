using BuyStuffOnline.DataAccess.Repository;
using BuyStuffOnline.DataAccess.Repository.IRepository;
using BuyStuffOnline.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BuyStuff.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _objProduct;
        private readonly ICategoryRepository _objCategory;
        public HomeController(ILogger<HomeController> logger, IProductRepository objProduct, ICategoryRepository objCategory)
        {
            _logger = logger;
            _objProduct = objProduct;
            _objCategory = objCategory;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> ProductList = _objProduct.GetAll(includeProperties:"Category");
            return View(ProductList);
        }


        public IActionResult Details(int ProductId) 
        { 
            if(ProductId == 0)
                return NotFound();

            Product product = _objProduct.Get(x => x.ID == ProductId, includeProperties:"Category");
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
