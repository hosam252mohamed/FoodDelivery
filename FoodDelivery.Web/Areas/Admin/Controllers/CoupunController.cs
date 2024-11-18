using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.Repository;
using Stripe; // Adjust as necessary

namespace FoodDelivery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CoupunController : Controller
    {
        private readonly ICoupunRepositoy _coupunRepo;

        public CoupunController(ICoupunRepositoy coupunRepo)
        {
            _coupunRepo = coupunRepo;
        }

        // List all coupons (Index)
        public IActionResult Index()
        {
            var coupuns = _coupunRepo.GetAll();  // Fetch all coupons
            return View(coupuns);  // Pass the coupons list to the view
        }

        // Create or Update Coupon (GET)
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            if (id is null || id == 0)
                return View(new Coupun());  // Create new coupon
            else
                return View(_coupunRepo.Get((int)id));  // Get existing coupon for update
        }

        // Create or Update Coupon (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Coupun coupun)
        {
            if (ModelState.IsValid)
            {
                if (coupun.CoupunID == 0)  // Create new coupon
                {
                    try
                    {

                        var options = new CouponCreateOptions { Currency = "egp", Name = coupun.Name, Id = coupun.Name };
                        if (coupun.Type == "Percent")
                            options.PercentOff = coupun.Discount;
                        else
                            options.AmountOff = (long)coupun.Discount * 100;
                        var service = new CouponService();
                        service.Create(options);
                        _coupunRepo.Add(coupun);
                        TempData["success"] = "Created Successfully";
                    }
                    catch(Exception ex)
                    {
                        if (ex.Message.Contains("regex"))
                        {
                            TempData["error"] = $"Error! Please don't add special characters";
                        }
                        else
                        {
                            TempData["error"] = $"There is Problem! {ex.Message}";
                        }
                    }

                }
                else  // Update existing coupon
                {
                    _coupunRepo.Update(coupun);
                    TempData["success"] = "Updated Successfully";
                }
                _coupunRepo.Save();
                return RedirectToAction("Index");
            }
            return View(coupun);  // Return the form with validation errors
        }

        // Delete Coupon (AJAX)
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            if (id == 0)
                return Json(new { success = false, message = "Error Operation Failed!" });

            var service = new CouponService();
            service.Delete(_coupunRepo.Get(id).Name);
            _coupunRepo.Remove(id);
            _coupunRepo.Save();
            return Json(new { success = true, message = "Deleted Successfully!" });
        }

        #region API Methods

        // Fetch all coupons as JSON (for data tables or AJAX)
        public IActionResult GetAll()
        {
            var coupuns = _coupunRepo.GetAll();
            return Json(new { data = coupuns });
        }

        // Activate or deactivate a coupon (AJAX)
        [HttpPost]
        public IActionResult Activate([FromBody] int id)
        {
            var coupun = _coupunRepo.Get(id);
            if (coupun == null)
                return Json(new { success = false, message = "Error: Can't Operate!" });

            coupun.IsActive = !coupun.IsActive;
            _coupunRepo.Update(coupun);
            _coupunRepo.Save();
            return Json(new { success = true, message = "Operation Successfully" });
        }

        #endregion
    }
}
