using FoodDelivery.Repository.Data;
using FoodDelivery.Web.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;

namespace FoodDelivery.Web.Areas.Main.Controllers
{
    [Area("Main")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _email;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext context, IEmailSender email, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _email = email;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Service()
        {
            return View();
        }
        public IActionResult Contact()
        {
            if (User.Identity.IsAuthenticated)
            {
                string email = _userManager.FindByNameAsync(User.Identity.Name).GetAwaiter().GetResult().Email;
                ViewBag.Email = email;
            }
            return View();
        }
        public IActionResult Booking()
        {
            return View();
        }
        public IActionResult Testimonial()
        {
            List<CustomerReview> Reviews = new List<CustomerReview>();
            if (User.Identity.IsAuthenticated)
            {
                //Logined User At Top
                string userId = _userManager.GetUserId(User);
                var userRevs = _context.CustomerReviews.Where(r => r.AppUserId == userId)
                    .Include(r => r.ApplicationUser).OrderByDescending(r => r.DatePublished).ToList();
                if (userRevs != null) Reviews.AddRange(userRevs);

                //Append The Rest
                var AllOtherRevs = _context.CustomerReviews.Where(r => r.AppUserId != userId)
                    .Include(r => r.ApplicationUser).OrderByDescending(r => r.DatePublished).ToList();
                if (AllOtherRevs != null) Reviews.AddRange(AllOtherRevs);
            }
            else
            {
                //If Not Logined
                var AllRevs = _context.CustomerReviews.Include(r => r.ApplicationUser).OrderByDescending(r => r.DatePublished).ToList();
                if (AllRevs != null) Reviews.AddRange(AllRevs);
            }
            return View(Reviews);
        }

        // POST: Create a new review
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Testimonial(CustomerReview review)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && !string.IsNullOrWhiteSpace(review.Text))
                {
                    review.AppUserId = user.Id;
                    review.DatePublished = DateTime.UtcNow;
                    _context.CustomerReviews.Add(review);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Testimonial"); 
        }

        // POST: Update the review
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(CustomerReview updatedReview)
        {
            var review = await _context.CustomerReviews.FindAsync(updatedReview.Id);
            if (review == null || review.AppUserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(updatedReview.Text))
            {
                review.Text = updatedReview.Text;
                review.DateEdited = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Testimonial");
        }

        // POST: Delete the review
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.CustomerReviews.FindAsync(id);
            var x = review.AppUserId != _userManager.GetUserId(User) && !User.IsInRole("Admin");
            if (review == null || review.AppUserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return Json(new {success = false, message = "You are not authorized"});
            }

            _context.CustomerReviews.Remove(review);
            await _context.SaveChangesAsync();
            return Json(new {success = true, message = "Deleted Successfully"});
        }
        public IActionResult Team()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EmailSubscribe([FromBody] string email)
        {
            try
            {
                new MailAddress(email);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = "Email Format Not Correct" });

            }
            if (_context.EmailSubscribers.FirstOrDefault(e => e.Email == email) is null)
            {
                EmailSubscriber emailSub = new() { Email = email };
                _context.Add(emailSub);
                _context.SaveChanges();

                var emailEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(email));
                var url = Request.Scheme + "://" + Request.Host.Value + $"/Main/Home/EmailUnsubscribe?email={emailEncoded}";
                var htmlMessage = 
                    "<div style='text-align: center; padding: 20px;'><h4>Thank You For Your Email Subscription, any new information will be sent to you continuously,</h4>" +
                    $"<p style='color: #666'>To Unsubscribe <a href='{HtmlEncoder.Default.Encode(url)}' style='text-decoration: none; font-style: italic; color: #0c46ac;'>Click Here</a></p></div>";
                _email.SendEmailAsync(email, "FoodWeb Subscription", 
                    htmlMessage).GetAwaiter().GetResult();

                return Json(new { success = true, message = "Email Is Subscribed Successfully" });
            }
            return Json(new { success = false, message = "Email Is Already Subscribed" });
        }
        public IActionResult EmailUnsubscribe(string email)
        {
            string decodedEmail;
            try { decodedEmail = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(email)); }
            catch (Exception ex) { return NotFound(); }
            if (_context.EmailSubscribers.FirstOrDefault(e => e.Email == decodedEmail) is not null)
            {
                _context.Remove(_context.EmailSubscribers.First(e => e.Email == decodedEmail));
                _context.SaveChanges();
                return View();
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Contact(ContactForm model)
        {
            if (ModelState.IsValid)
            {
                var subject = model.Subject;
                var message = $"Name: {model.Name}<br>Email: {model.Email}<br>Message: {model.Message}";
                try
                {
                    await _email.SendEmailAsync(model.Email, subject, message);
                    _context.ContactForms.Add(model);
                    _context.SaveChanges();
                    TempData["success"]= "Your message has been sent successfully!";                }
                catch (Exception ex)
                {
                    TempData["error"]= "There was an error sending your message.";    
                }
            }
            else
                TempData["error"] = "There was an error sending your message.";

            return RedirectToAction("Contact");
        }
    }
}
