using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;
using FoodDelivery.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Linq;
using System.Security.Claims;

namespace FoodDelivery.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public BookingController(ApplicationDbContext context,UserManager<IdentityUser> userManager, IEmailSender email)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = email;
        }

        // GET: Booking Form
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string email = _userManager.FindByNameAsync(User.Identity.Name).GetAwaiter().GetResult().Email;
                ViewBag.Email = email;
            }
            return View();
        }

        // POST: Submit Booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var booking = new Booking
                {
                    CustomerName = model.Name,
                    Email = model.Email,
                    DateAndTime = model.BookingDate,
                    NumberOfPeople = model.NumberOfPeople,
                    SpecialRequest = model.SpecialRequest,
                    Confirmed = false,
                    Status = "Pending" // Set default status
                };

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                TempData["success"] = "Booking Successful!";
                return RedirectToAction("Confirmation",new {id = booking.Id});
            }
            return View(model);
        }

        // GET: Booking Confirmation
        public IActionResult Confirmation(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking != null && booking.Confirmed == false) {
                booking.Confirmed = true; 
                _context.SaveChanges();
                _emailSender.SendEmailAsync(booking.Email, "Booking A Table", $"You Have Booked New Table Successfully #{id}").GetAwaiter().GetResult();
                return View("Confirmation",true);
            }
            return View(false);
        }

        // GET: My Bookings (Customer View)
        [Authorize(Roles = "Customer")]
        public IActionResult MyBookings(string status)
        {
            // Get the current customer's email from the User identity
            string userEmail = _userManager.FindByNameAsync(User.Identity.Name).GetAwaiter().GetResult().Email;

            var bookings = _context.Bookings
                .Where(b => b.Email == userEmail);

            // Filter based on status if provided
            if (!string.IsNullOrEmpty(status))
            {
                bookings = bookings.Where(b => b.Status == status);
            }

            return View(bookings.ToList());
        }

        // POST: Cancel Booking
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status == "Pending")
            {
                booking.Status = "Canceled";
                _context.SaveChanges();
                return Json(new { success = true, message = "Your booking has been canceled" });
            }
            return Json(new { success = false, message = "Only pending bookings can be canceled." });
        }
    }
}
