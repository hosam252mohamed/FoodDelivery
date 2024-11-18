using FoodDelivery.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodDelivery.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ApplicationDbContext _context;

        public ProductController(IProductRepository productRepository, ApplicationDbContext context)
        {
            _productRepo = productRepository;
            _context = context;
        }
        public IActionResult Index(string search, int? categoryId, int page = 1, int pageSize = 8)
        {
            ViewBag.Categories = _context.Categories.ToList();
            // Fetch products with filtering
            var productsQuery = _context.Products
                                        .Include(p => p.Category)
                                        .Include(p => p.ProductImages)
                                        .AsQueryable();

            // Filter by category
            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }

            // Search by product name
            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(search));
            }

            // Paging
            var totalProducts = productsQuery.Count();
            var products = productsQuery.Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

            // Create a paged result
            var result = new
            {
                TotalItems = totalProducts,
                Products = products,
                PageSize = pageSize
            };

            // If it's an AJAX request, return JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(result);
            }

            return View(result);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (id != 0)
            {
                Cart cart = new()
                {
                    Product = _context.Products.Include(p => p.Category).Include(p => p.ProductImages).FirstOrDefault(p => p.ProductID == id),
                    ProductID = id,
                    Count = 1,
                };
                return View(cart);
            }
            return RedirectToAction("Index");
        }
		[HttpPost]
		[Authorize(Roles = "Customer")]
		public IActionResult Details(Cart cart)
		{
			if (ModelState.IsValid)
			{
				var claimIdentity = (ClaimsIdentity)User.Identity;
				var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
				cart.ApplicationUserId = claims.Value;
				var cartDb = _context.Carts.Where(c => c.ApplicationUserId == cart.ApplicationUserId && c.ProductID == cart.ProductID).FirstOrDefault();
                if (cartDb is null)
                    _context.Add(cart);
                else
                {
                    cartDb.Count += cart.Count;
                    if (cartDb.Count >= 50) cartDb.Count = 50;
                }
				_context.SaveChanges();
				//Set Cart Session
				HttpContext.Session.SetInt32("CartSession", _context.Carts.Where(c => c.ApplicationUserId == claims.Value).ToList().Count);

				var productName = _context.Products.FirstOrDefault(p => p.ProductID == cart.ProductID).Name;
				TempData["success"] = $"{productName} Added Successfully To The Cart";
			}
			return RedirectToAction("Details", cart.ProductID);
		}
	}
}
