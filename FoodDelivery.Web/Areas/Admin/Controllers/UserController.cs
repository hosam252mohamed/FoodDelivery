using FoodDelivery.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FoodDelivery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IApplicationUserRepository appUser;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ApplicationDbContext context, IApplicationUserRepository appUser, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.appUser = appUser;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = appUser.Get(claim.Value);
            return View(user);
        }
        [HttpGet]
        public IActionResult Permission(string id)
        {
            var RolesList = _roleManager.Roles.Select(i => new SelectListItem(i.Name, i.Name)).ToList();
            var user = appUser.Get(id);
            UserRolesVM userRolesVM = new UserRolesVM
            {
                Roles = RolesList,
                User = user
            };
            return View(userRolesVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Permission(UserRolesVM userVM)
        {
            var user = appUser.Get(userVM.User.Id);
            appUser.Update(user, userVM.User.Role);
            return RedirectToAction("Index");
        }

        #region APIS
        public IActionResult GetAll()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = appUser.Get(claim.Value);
            return Json(new { data = appUser.GetAll(user) });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {

            var objFromDb = appUser.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Problem With Account!" });
            }
            //Unlock
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.UtcNow)
            {
                
                objFromDb.LockoutEnd = DateTime.UtcNow;
            }
            //Lock
            else
            {
                objFromDb.LockoutEnd = DateTime.UtcNow.AddYears(1000);
            }
            _context.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }
        #endregion
    }
}
