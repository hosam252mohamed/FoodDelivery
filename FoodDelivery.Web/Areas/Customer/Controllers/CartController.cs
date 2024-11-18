using FoodDelivery.Models;
using FoodDelivery.Repository.Data;
using FoodDelivery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using NuGet.Protocol;
using Stripe.Checkout;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;

namespace FoodDelivery.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartRepository _cartRepo;
        private readonly IServiceProvider _service;
        private readonly IOrderHeaderRepository _headerRepo;
        private readonly IEmailSender _emailSender;

        public CartController(
            ApplicationDbContext context,
            ICartRepository cartRepo,
            IServiceProvider service,
            IOrderHeaderRepository headerRepo,
            IEmailSender emailSender)
        {
            _context = context;
            _cartRepo = cartRepo;
            _service = service;
            _headerRepo = headerRepo;
            _emailSender = emailSender;
        }
        [BindProperty]
        public CartViewModel CartVM { get; set; } = new CartViewModel();
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ////remove pending payment orders
            var orders = _context.OrderHeaders
                                 .Where(o => o.AppUserId == claims.Value && o.PaymentStatus == "Pending")
                                 .ToList();
            if (orders != null) { _context.RemoveRange(orders); _context.SaveChanges(); }
            ////
            
            CartVM.CartsList = _cartRepo.GetAll(claims.Value, "Product");
            CartVM.orderHeader.SubTotal = _cartRepo.CalculateTotalPrice(claims.Value);
            CartVM.orderHeader.OrderTotal = _cartRepo.CalculateTotalPrice(claims.Value);
            return View(CartVM);
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            CartVM.CartsList = _cartRepo.GetAll(claims.Value, "Product");
            CartVM.orderHeader.SubTotal = _cartRepo.CalculateTotalPrice(claims.Value);
            CartVM.orderHeader.OrderTotal = CartVM.orderHeader.SubTotal;
            CartVM.ShippingInfo = _context.ShippingInfos.FirstOrDefault(s => s.AppUserId == claims.Value);

            return View(CartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST(CartViewModel carvm)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claims.Value;

            // Check if cart is empty
            if (_cartRepo.GetAll(claims.Value).Count == 0)
            {
                TempData["error"] = "Please Add Items To the Cart";
                return RedirectToAction("Summary");
            }

            var cartList = _cartRepo.GetAll(userId, "Product");
            CartVM.CartsList = cartList;
            CartVM.orderHeader.AppUserId = userId;
            CartVM.orderHeader.OrderStatus = Status.OrderStatusPending;
            CartVM.orderHeader.PaymentStatus = Status.PaymentStatusPending;
            CartVM.orderHeader.SubTotal = _cartRepo.CalculateTotalPrice(userId);
            CartVM.orderHeader.OrderTotal = CartVM.orderHeader.SubTotal;

            // Re-apply the coupon if available
            if(carvm.orderHeader.CoupunCode != null) 
                ApplyCoupon(carvm.orderHeader.CoupunCode);
            // Save order information to the database
            if (_context.ShippingInfos.FirstOrDefault(s => s.AppUserId == userId) == null)
            {
                carvm.ShippingInfo.AppUserId = claims.Value;
                _context.Add(carvm.ShippingInfo);
                _context.SaveChanges();
            }

            CartVM.orderHeader.shippingInfoId = _context.ShippingInfos
                .FirstOrDefault(s => s.AppUserId == userId).Id;

            _context.Add(CartVM.orderHeader);
            _context.SaveChanges();

            foreach (var cart in cartList)
            {
                OrderDetails ordDet = new()
                {
                    OrderHeaderId = CartVM.orderHeader.Id,
                    ProductId = cart.ProductID,
                    Price = cart.Product.Price,
                    Quantity = cart.Count
                };
                _context.Add(ordDet);
                _context.SaveChanges();
            }
            // Redirect to payment session creation...
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"customer/cart/OrderConfirmation/{CartVM.orderHeader.Id}",
                CancelUrl = domain + "customer/cart/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = _context.ApplicationUsers.First(u => u.Id == userId).Email,
            };

            foreach (var item in CartVM.CartsList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }
            if (CartVM.orderHeader.CoupunCode != null) options.Discounts = new() { new() { Coupon = CartVM.orderHeader.CoupunCode } };

            var service = new SessionService();
            Session session = service.Create(options);
            _headerRepo.UpdatePaymentID(CartVM.orderHeader.Id, session.Id, session.PaymentIntentId);
            _headerRepo.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _headerRepo.Get(id, "User");
            var service = new SessionService();
            var session = service.Get(orderHeader.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                if(orderHeader.OrderTotal == 0)
                    _headerRepo.UpdateStatus(id, Status.OrderStatusInProcess, Status.PaymentStatusFree);
                else
                    _headerRepo.UpdateStatus(id, Status.OrderStatusInProcess, Status.PaymentStatusApproved);
                _headerRepo.UpdatePaymentID(id, session.Id, session.PaymentIntentId);
                orderHeader.OrderDate = DateTime.UtcNow;

                _headerRepo.Save();
            }
            HttpContext.Session.Clear();

            var productsHTML = "";
            var productsList = _cartRepo.GetAll(orderHeader.AppUserId, "Product");
            foreach (var item in productsList)
            {
                productsHTML += $"<div style='display: flex'><div style='text-align: start'><h3 style='margin: 0; margin-right: 20px;  margin-bottom: 1vh; text-align: start;'>{item.Product.Name}</h3><span>Quantity: {item.Count}</span></div><span>${item.Product.Price * item.Count}</span></div> <hr />";
            }
            string coupun = "";
            if(orderHeader.CoupunCode != null )
                if(orderHeader.CoupunType == "Percent")
                    coupun = $"%{Math.Round(orderHeader.CoupunDiscount)}";
                else
					coupun = $"${orderHeader.CoupunDiscount}";
			var htmlMessage =
                    $"<div style='text-align: center; padding: 20px; width: 260px; margin: 0 auto;'><h4>Thank You For Your Order #{orderHeader.Id}</h4> <hr />" +
                    $"<div style='text-align: center'>{productsHTML}</div>" +
					$"<div style='display: flex'><h3 style='margin: 0; margin-right: 20px; text-align: start;'>SubTotal</h3><span>${orderHeader.SubTotal}</span></div>  <hr />" +
					(orderHeader.CoupunCode != null ? $"<div style='display: flex'><h3 style='margin: 0; margin-right: 20px; text-align: start;'>Coupon Applied: </h3><span>{coupun}</span></div> <hr />" : "") +
					$"<div style='display: flex'><h3 style='margin: 0; margin-right: 20px; text-align: start;'>OrderTotal</h3><span>${orderHeader.OrderTotal}</span></div>";
            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, $"Order Placed #{orderHeader.Id}", htmlMessage).GetAwaiter().GetResult();

            _context.RemoveRange(productsList);
            _context.SaveChanges();

            TempData["success"] = "Order Placed Successfully";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAddress(ShippingInfo shippingInfo)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var ShippingAddress = _context.ShippingInfos.FirstOrDefault(s => s.AppUserId == claims.Value);
            if (ShippingAddress is null)
            {
                shippingInfo.AppUserId = claims.Value;
                _context.Add(shippingInfo);
                TempData["success"] = "Address Saved Successfully";
            }
            else
            {
                ShippingAddress.Name = shippingInfo.Name;
                ShippingAddress.Government = shippingInfo.Government;
                ShippingAddress.City = shippingInfo.City;
                ShippingAddress.PostalCode = shippingInfo.PostalCode;
                ShippingAddress.NearestPlace = shippingInfo.NearestPlace;
                ShippingAddress.FullAddress = shippingInfo.FullAddress;
                ShippingAddress.Phone = shippingInfo.Phone;
                _context.Update(ShippingAddress);
                TempData["success"] = "Address Updated Successfully";
            }
            _context.SaveChanges();
            return RedirectToAction("Summary");
        }

        #region APIS
        [HttpPost]
        public async Task<IActionResult> Plus([FromBody] int id)
        {
            var productCart = await _context.Carts.FirstOrDefaultAsync(c => c.CartID == id);
            if(productCart.Count == 50)
				return Json(new { success = false, message = "Failed" });
			productCart.Count += 1;
            _context.Update(productCart);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetInt32("CartSession", _context.Carts.Where(c => c.ApplicationUserId == productCart.ApplicationUserId).Count());
            return Json(new { success = true, message = "Operation Done" });
        }
        [HttpPost]
        public async Task<IActionResult> Minus([FromBody] int id)
        {
            var productCart = await _context.Carts.FirstOrDefaultAsync(c => c.CartID == id);
            if (productCart.Count == 1) return Json(new { success = false, message = "" });
            productCart.Count -= 1;
            _context.Update(productCart);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetInt32("CartSession", _context.Carts.Where(c => c.ApplicationUserId == productCart.ApplicationUserId).Count());
            return Json(new { success = true, message = "Operation Done" });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var productCart = await _context.Carts.FirstOrDefaultAsync(c => c.CartID == id);
            _context.Remove(productCart);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetInt32("CartSession", _context.Carts.Where(c => c.ApplicationUserId == productCart.ApplicationUserId).Count());
            return Json(new { success = true, message = "Operation Done" });
        }
        [HttpPost]
        public IActionResult ApplyCoupon(string couponName)
        {
            var coupon = _context.Coupuns.FirstOrDefault(c => c.Name == couponName.ToLower());
            if (coupon == null || !coupon.IsActive || coupon.ValidTo < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                return Json(new { success = false, message = "Invalid or expired coupon" });
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            CartVM.CartsList = _cartRepo.GetAll(claims.Value, "Product");
            CartVM.orderHeader.SubTotal = _cartRepo.CalculateTotalPrice(claims.Value);

            if (CartVM.orderHeader.SubTotal < coupon.MinAmount && !(coupon.Discount == 100 && coupon.Type == "Percent"))
            {
                return Json(new { success = false, message = $"Mininmun order amount should be {coupon.MinAmount}" });
            }

            CartVM.orderHeader.CoupunCode = coupon.Name;
            CartVM.orderHeader.CoupunDiscount = coupon.Discount;
            CartVM.orderHeader.CoupunType = coupon.Type;

            if (coupon.Type == "Percent")
            {
                CartVM.orderHeader.OrderTotal = CartVM.orderHeader.SubTotal - (CartVM.orderHeader.SubTotal * (coupon.Discount / 100));
            }
            else
            {
                CartVM.orderHeader.OrderTotal = CartVM.orderHeader.SubTotal - coupon.Discount;
            }

            return Json(new { success = true, Name = couponName, newTotal = CartVM.orderHeader.OrderTotal,current = CartVM.orderHeader.SubTotal, message = $"{coupon.Name} is applied" });
        }
        #endregion
    }
}
