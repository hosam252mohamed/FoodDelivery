using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels; // Add this using directive
using FoodDelivery.Repository.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FoodDelivery.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Stripe.Checkout;
using Stripe;


namespace FoodDelivery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: All Orders
        public IActionResult Index(string search, string status)
        {
            var orders = _context.OrderHeaders.Include(o => o.ApplicationUser).Include(o=>o.shippingInfo).Where(o=>o.PaymentStatus != "Pending").AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(search))
            {
                orders = orders.Where(o => o.ApplicationUser.UserName.Contains(search) || o.Id.ToString().Contains(search));
            }

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.OrderStatus == status);
            }

            return View(orders.ToList());
        }

        public IActionResult Details(int id)
        {
            var orderHeader = _context.OrderHeaders
                .Include(o => o.ApplicationUser)
                .Include(o => o.shippingInfo)
                .FirstOrDefault(o => o.Id == id);

            if (orderHeader == null)
            {
                return NotFound();
            }

            var orderDetails = _context.OrderDetails
                .Include(d => d.Product)
                .Where(d => d.OrderHeaderId == id)
                .ToList();

            var viewModel = new OrderDetailsViewModel
            {
                OrderHeader = orderHeader,
                OrderDetails = orderDetails
            };

            // Ensure Quantity and Price values are correct at this stage
            foreach (var detail in orderDetails)
            {
                System.Diagnostics.Debug.WriteLine($"Product: {detail.Product.Name}, Quantity: {detail.Quantity}, Price: {detail.Price}");
            }

            return View(viewModel);
        }


        // GET: Edit Order Status
        [HttpGet]
        public IActionResult EditStatus(int id)
        {
            var order = _context.OrderHeaders.FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Update Order Status
        [HttpPost]
        public IActionResult EditStatus(int id, string status)
        {
            var order = _context.OrderHeaders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            if(status == Status.OrderStatusCancelled)
            {
                if (order.OrderTotal != 0)
                {
                    var refund = new RefundCreateOptions() { PaymentIntent = order.TransactionId, Reason = RefundReasons.RequestedByCustomer };
                    var service = new RefundService();
                    service.Create(refund);
                    order.PaymentStatus = Status.OrderStatusRefunded;
                }
                else 
                    order.PaymentStatus = Status.PaymentStatusFree;

            }
            order.OrderStatus = status;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // DELETE: Order
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var orderHeader = _context.OrderHeaders.FirstOrDefault(o => o.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            var orderDetails = _context.OrderDetails.Where(d => d.OrderHeaderId == id).ToList();
            _context.OrderDetails.RemoveRange(orderDetails);
            _context.OrderHeaders.Remove(orderHeader);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
