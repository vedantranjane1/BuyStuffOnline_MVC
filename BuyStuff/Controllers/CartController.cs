using BuyStuffOnline.DataAccess.Repository.IRepository;
using BuyStuffOnline.Models;
using BuyStuffOnline.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BuyStuff.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        public readonly IShoppingCartRepository _cartRepository;
        public CartController(IShoppingCartRepository objCartRepo)
        {
            _cartRepository = objCartRepo;
        }
        public IActionResult Index()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM();
            shoppingCartVM.CartList = _cartRepository.GetAll(x => x.ApplicationUserID == userId, includeProperties: "Product");

            foreach (ShoppingCart cart in shoppingCartVM.CartList)
            {
                if (cart.Count < 10)
                    cart.Price = cart.Product.ListPrice;
                else if (cart.Count < 50)
                    cart.Price = cart.Product.Price;
                else
                    cart.Price = cart.Product.Price50;


                shoppingCartVM.OrderTotal += cart.Count * cart.Price;
            }
            return View(shoppingCartVM);
        }

        public ActionResult AddQty(int? ID)
        {
            if (ID == 0 || ID == null)
                return NotFound();

            ShoppingCart? shoppingCart = _cartRepository.Get(x => x.ID == ID);

            if (shoppingCart == null)
                return NotFound();

            shoppingCart.Count += 1;
            _cartRepository.Update(shoppingCart);
            _cartRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult SubQty(int? ID)
        {
            if (ID == 0 || ID == null)
                return NotFound();

            ShoppingCart? shoppingCart = _cartRepository.Get(x => x.ID == ID);

            if (shoppingCart == null)
                return NotFound();

            shoppingCart.Count -= 1;


            if (shoppingCart.Count <= 0)
                _cartRepository.Remove(shoppingCart);
            else
                _cartRepository.Update(shoppingCart);

            _cartRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Remove(int? ID)
        {
            if(ID == 0 || ID == null)
                return NotFound();

            ShoppingCart? shoppingCart = _cartRepository.Get(x => x.ID == ID);

            if (shoppingCart == null)
                return NotFound();

            _cartRepository.Remove(shoppingCart);
            _cartRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Summary()
        {
            return View();
        }
    }
}
