using BuyStuffOnline.DataAccess.Repository;
using BuyStuffOnline.DataAccess.Repository.IRepository;
using BuyStuffOnline.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BuyStuff.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _objProduct;
        private readonly ICategoryRepository _objCategory;
        private readonly IShoppingCartRepository _objShoppingCart;
        public HomeController(ILogger<HomeController> logger, IProductRepository objProduct, ICategoryRepository objCategory, IShoppingCartRepository objShoppingCart)
        {
            _logger = logger;
            _objProduct = objProduct;
            _objCategory = objCategory;
            _objShoppingCart = objShoppingCart;
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
            ShoppingCart objCart = new ShoppingCart();
            objCart.Product = product;
            objCart.Count = 1;
            return View(objCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserID = userId;

            ShoppingCart CartFromDB = _objShoppingCart.Get(x => x.ProductID == shoppingCart.ProductID && x.ApplicationUserID == shoppingCart.ApplicationUserID);

            if (CartFromDB != null)
            {
                CartFromDB.Count += shoppingCart.Count;
                _objShoppingCart.Update(CartFromDB);
            }
            else 
            {
                _objShoppingCart.Add(shoppingCart);
            }

            _objShoppingCart.Save();
            TempData["success"] = "Added To Cart!!!";
            return Redirect(nameof(Index));
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
