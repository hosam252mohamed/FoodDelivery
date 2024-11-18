using FoodDelivery.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Claims;

namespace FoodDelivery.Web.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public CartViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(HttpContext.Session.GetInt32("CartSession") is null)
            {
                HttpContext.Session.SetInt32("CartSession", _context.Carts.Where(c => c.ApplicationUserId == claims.Value).ToList().Count);
                return View(HttpContext.Session.GetInt32("CartSession"));
            }
            return View(HttpContext.Session.GetInt32("CartSession"));
        }
    }
}
