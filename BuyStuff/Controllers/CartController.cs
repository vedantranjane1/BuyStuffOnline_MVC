using Azure.Core;
using BuyStuffOnline.DataAccess.Repository;
using BuyStuffOnline.DataAccess.Repository.IRepository;
using BuyStuffOnline.Models;
using BuyStuffOnline.Models.ViewModels;
using BuyStuffOnline.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BuyStuff.Controllers
{
	[Authorize]
	public class CartController : Controller
	{
		public readonly IShoppingCartRepository _cartRepository;
		public readonly IApplicationUserRepository _userRepository;
		public readonly IOrderHeaderRepository _orderHeaderRepository;
		public readonly IOrderDetailRepository _orderDetailRepository;
		public readonly IShoppingCartRepository _shoppingCartRepository;

		[BindProperty]
		ShoppingCartVM ShoppingCartVM { get; set; }

		public CartController(IShoppingCartRepository objCartRepo, IApplicationUserRepository objAppUserRepo, IOrderHeaderRepository headerRepository, IOrderDetailRepository orderDetailRepository,IShoppingCartRepository shoppingCartRepository)
		{
			_cartRepository = objCartRepo;
			_userRepository = objAppUserRepo;
			_orderHeaderRepository = headerRepository;
			_orderDetailRepository = orderDetailRepository;
			_shoppingCartRepository = shoppingCartRepository;
		}
		public IActionResult Index()
		{
			ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM = new ShoppingCartVM();
			ShoppingCartVM.CartList = _cartRepository.GetAll(x => x.ApplicationUserID == userId, includeProperties: "Product");
			ShoppingCartVM.OrderHeader = new();

			foreach (ShoppingCart cart in ShoppingCartVM.CartList)
			{
				if (cart.Count < 10)
					cart.Price = cart.Product.ListPrice;
				else if (cart.Count < 50)
					cart.Price = cart.Product.Price;
				else
					cart.Price = cart.Product.Price50;


				ShoppingCartVM.OrderHeader.OrderTotal += (double)(cart.Count * cart.Price);
			}
			return View(ShoppingCartVM);
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
			if (ID == 0 || ID == null)
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
			ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM = new()
			{
				CartList = _cartRepository.GetAll(x => x.ApplicationUserID == userId, includeProperties: "Product"),
				OrderHeader = new()
			};
			ShoppingCartVM.OrderHeader.ApplicationUser = _userRepository.Get(x => x.Id == userId);
			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.Street = ShoppingCartVM.OrderHeader.ApplicationUser.Street;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;




			foreach (ShoppingCart cart in ShoppingCartVM.CartList)
			{
				if (cart.Count < 10)
					cart.Price = cart.Product.ListPrice;
				else if (cart.Count < 50)
					cart.Price = cart.Product.Price;
				else
					cart.Price = cart.Product.Price50;


				ShoppingCartVM.OrderHeader.OrderTotal += (double)(cart.Count * cart.Price);
			}

			return View(ShoppingCartVM);
		}


		[HttpPost]
		[ActionName("Summary")]
		public ActionResult SummaryPOST(ShoppingCartVM ShoppingCartVM)
		{
			ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM.CartList = _cartRepository.GetAll(x => x.ApplicationUserID == userId, includeProperties: "Product");

			ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			ApplicationUser applicationUser = _userRepository.Get(x => x.Id == userId);



			foreach (ShoppingCart cart in ShoppingCartVM.CartList)
			{
				if (cart.Count < 10)
					cart.Price = cart.Product.ListPrice;
				else if (cart.Count < 50)
					cart.Price = cart.Product.Price;
				else
					cart.Price = cart.Product.Price50;


				ShoppingCartVM.OrderHeader.OrderTotal += (double)(cart.Count * cart.Price);
			}

			if (applicationUser.CompanyID.GetValueOrDefault() == 0)
			{
				//it is a normal customer and he needs to make the payment
				ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;

			}
			else
			{
				//it is a company user
				ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
			}

			_orderHeaderRepository.Add(ShoppingCartVM.OrderHeader);
			_orderHeaderRepository.Save();

			foreach (ShoppingCart cart in ShoppingCartVM.CartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductID,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					Price = (double)cart.Price,
					Count = cart.Count,

				};
				_orderDetailRepository.Add(orderDetail);
				_orderDetailRepository.Save();

			}

			if (applicationUser.CompanyID.GetValueOrDefault() == 0)
			{
				//it is a normal customer and he needs to make the payment
				//stripe logic
				var domain = "https://localhost:7023/";
				var options = new SessionCreateOptions
				{
					SuccessUrl = domain + $"cart/OrderConfirmation?Id={ShoppingCartVM.OrderHeader.Id}",
					CancelUrl = domain + $"cart/Index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

				foreach (var item in ShoppingCartVM.CartList)
				{
					var sessionLineItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.Price * 100),
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Product.Title
							}
						},
						Quantity = item.Count
					};
					options.LineItems.Add(sessionLineItem);

				}



				var service = new SessionService();
				Session session = service.Create(options);

				_orderHeaderRepository.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
				_orderHeaderRepository.Save();

				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);

			}

			return RedirectToAction(nameof(OrderConfirmation), new { ShoppingCartVM.OrderHeader.Id });
		}

		public IActionResult OrderConfirmation(int Id)
		{
			OrderHeader orderHeader = _orderHeaderRepository.Get(x => x.Id == Id,includeProperties:"ApplicationUser");

			if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
			{//Normal Customer 
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session != null && session.PaymentStatus.ToLower() == "paid")
				{
					_orderHeaderRepository.UpdateStripePaymentID(Id, session.Id, session.PaymentIntentId);
					_orderHeaderRepository.UpdateStatus(Id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
					_orderHeaderRepository.Save();
				}
			}
			List<ShoppingCart> shoppingCarts = _shoppingCartRepository.GetAll(x=>x.ApplicationUserID == orderHeader.ApplicationUserId).ToList();

			_shoppingCartRepository.RemoveRange(shoppingCarts);


			return View(Id);
		}
	}
}
