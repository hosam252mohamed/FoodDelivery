using FoodDelivery.Models;
using FoodDelivery.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FoodDelivery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Display all bookings
        public IActionResult Index()
        {
            var bookings = _context.Bookings.ToList();
            return View(bookings);
        }

        // GET: Edit Booking Status
        public IActionResult Edit(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // POST: Edit Booking Status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var bookingInDb = _context.Bookings.Find(id);
                if (bookingInDb == null)
                {
                    return NotFound();
                }

                // Update booking status
                bookingInDb.Status = booking.Status;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(o => o.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            _context.Remove(booking);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
