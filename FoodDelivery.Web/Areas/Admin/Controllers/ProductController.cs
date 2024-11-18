using FoodDelivery.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;
using System.IO;
using System.Linq;

namespace FoodDelivery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
   // [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment _web;

        public ProductController(IProductRepository productRepository, ApplicationDbContext _context, IWebHostEnvironment web)
        {
            this.productRepository = productRepository;
            context = _context;
            this._web = web;
        }

        // GET: ProductController
        public IActionResult Index()
        {
            return View(productRepository.GetAll("Category"));
        }

        // GET: ProductController/Upsert
        [HttpGet]
        public IActionResult Upsert(int id)
        {
            ViewBag.Categories = context.Categories.ToList();
            ProductVM productVM = new ProductVM();

            // Load existing product for editing
            if (id != 0)
            {
                Product product = productRepository.Get(id);
                if (product != null)
                {
                    productVM.ProductID = product.ProductID;
                    productVM.Name = product.Name;
                    productVM.Price = product.Price;
                    productVM.Description = product.Description;
                    productVM.CategoryId = product.CategoryId;
                }
                else
                {
                    return NotFound();  // If the product is not found, return 404
                }
            }
            return View(productVM);
        }

        // POST: ProductController/Upsert
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM)
        {
            ViewBag.Categories = context.Categories.ToList();

            if (ModelState.IsValid)
            {
                Product product;

                // Creating a new product
                if (productVM.ProductID == 0)
                {
                    product = new Product
                    {
                        Name = productVM.Name,
                        Price = productVM.Price,
                        Description = productVM.Description,
                        CategoryId = productVM.CategoryId,
                    };
                    productRepository.Add(product);
                    productRepository.Save();
                }
                else // Updating existing product
                {
                    product = productRepository.Get(productVM.ProductID);
                    if (product == null)
                    {
                        ModelState.AddModelError("", "Product not found.");
                        return View(productVM);
                    }

                    // Update product properties
                    product.Name = productVM.Name;
                    product.Price = productVM.Price;
                    product.Description = productVM.Description;
                    product.CategoryId = productVM.CategoryId;

                    // Handle image deletion for existing product
                    var existingImages = context.ProductImages.Where(i => i.ProductId == product.ProductID).ToList();
                    foreach (var img in existingImages)
                    {
                        var path = Path.Combine(_web.WebRootPath, img.ImageUrl);
                        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                        context.Remove(img);  // Remove image from the database
                    }

                    productRepository.Update(product);
                    productRepository.Save();  // Save the updated product before handling images
                }

                // Handle file uploads (images)
                var imgs = new List<ProductImages>();
                string[] allowedExt = { ".jpg", ".png", ".jpeg" };
                double MaxInMB = 2 * 1024 * 1024;
                bool isValid = true;

                foreach (var file in productVM.files)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (allowedExt.Contains(extension))
                    {
                        if (file.Length <= MaxInMB)
                        {
                            string randomName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".jpg";
                            string path = Path.Combine(_web.WebRootPath, "Images", randomName);

                            // Save image to server
                            using (FileStream stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            ProductImages productImage = new ProductImages
                            {
                                ImageUrl = $"Images/{randomName}",
                                ProductId = product.ProductID,
                            };
                            imgs.Add(productImage);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Image size should be less than 2MB.");
                            if (product.ProductID == 0) productRepository.Delete(product);
                            return View(productVM);
                        }
                    }
                    else
                    {
                        isValid = false;
                        break;
                    }
                }

                if (!isValid)
                {
                    ModelState.AddModelError("", "File type is not valid.");
                    if (product.ProductID == 0) productRepository.Delete(product);
                    return View(productVM);
                }

                // Add uploaded images to the product
                product.ProductImages.AddRange(imgs);
                productRepository.Update(product);
                productRepository.Save();

                return RedirectToAction("Index");
            }

            return View(productVM);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var product = productRepository.Get(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Delete associated images
            var prodImgs = context.ProductImages.Where(i => i.ProductId == id).ToList();
            foreach (var item in prodImgs)
            {
                string path = Path.Combine(_web.WebRootPath, item.ImageUrl);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            productRepository.Delete(product);
            productRepository.Save();
            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #region APIS

        public IActionResult GetAll()
        {
            var products = productRepository.GetAll("Category");
            return Json(new { data = products });
        }

        public IActionResult GetImages(int id)
        {
            var images = context.ProductImages.Where(p => p.ProductId == id).ToList();
            return PartialView("_GetImages", images);
        }

        #endregion
    }
}
