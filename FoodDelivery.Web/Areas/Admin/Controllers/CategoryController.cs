using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _category;
        public CategoryController(ICategoryRepository category)
        {
            _category = category;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(_category.GetAll());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _category.Add(category);
                _category.Save();
                return RedirectToAction("Index");
            }
            return View("Create", category);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(_category.GetById(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _category.Update(category);
                _category.Save();
                return RedirectToAction("Index");
            }
            return View("Edit", category);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(_category.GetById(id));
        }
        [HttpPost]
        public IActionResult Delete(Category category)
        {
            if (category.CategoryID != 0)
            {
                _category.Remove(category);
                _category.Save();
                TempData["Success"] = "Deleted Successfully";
            }
            else ModelState.AddModelError("", "Cannot Delete This Category");
            return RedirectToAction("Index");
        }
    }
}
