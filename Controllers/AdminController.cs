using CyberGuardian360.DBContext;
using CyberGuardian360.Models;
using CyberGuardian360.Models.EFDBContext;
using Microsoft.AspNetCore.Mvc;

namespace CyberGuardian360.Controllers
{
    public class AdminController : Controller
    {
        private readonly CyberGuardian360DbContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(CyberGuardian360DbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var products = _context.CSProducts.OrderBy(p => p.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalProducts = _context.CSProducts.Count();

            var model = new Pagination<CSProducts>(products, totalProducts, page, pageSize);

            return View(model);
        }

        public IActionResult Create()
        {
            TempData["AddSuccess"] = null;
            TempData["AddError"] = null;
            TempData["UpdateSuccess"] = null;
            TempData["UpdateError"] = null;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CSProducts product, IFormFile? ImageUrl)
        {
            TempData["AddSuccess"] = null;
            TempData["AddError"] = null;

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageUrl is { Length: > 0 })
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageUrl.FileName);

                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "product_images");

                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        await using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageUrl.CopyToAsync(stream);
                        }

                        product.ImageUrl = uniqueFileName;
                    }

                    product.CreatedDate = DateTime.Now;
                    product.ModifiedDate = DateTime.Now;

                    _context.CSProducts.Add(product);
                    await _context.SaveChangesAsync();

                    TempData["toastMsg"] = "Product Created.";

                    TempData["AddSuccess"] = $"Product {product.Id} is added to Menu.";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["AddError"] = $"Exception while adding the new Product : {ex.Message}";
                }
            }

            return View(product);
        }

        public IActionResult Edit(int id)
        {
            TempData["AddSuccess"] = null;
            TempData["AddError"] = null;
            TempData["UpdateSuccess"] = null;
            TempData["UpdateError"] = null;
            var product = _context.CSProducts.Find(id);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CSProducts csproducts, IFormFile? newImage)
        {
            TempData["UpdateSuccess"] = null;
            TempData["UpdateError"] = null;

            if (id != csproducts.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.CSProducts.FindAsync(id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    if (newImage != null)
                    {
                        if (existingProduct.ImageUrl != null)
                        {
                            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "product_images", existingProduct.ImageUrl);
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + newImage.FileName;

                        var newImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "product_images", uniqueFileName);
                        await using (var stream = new FileStream(newImagePath, FileMode.Create))
                        {
                            await newImage.CopyToAsync(stream);
                        }

                        existingProduct.ImageUrl = uniqueFileName;
                    }

                    existingProduct.ProductName = csproducts.ProductName;
                    existingProduct.ProductCost = csproducts.ProductCost;
                    existingProduct.ProductDescription = csproducts.ProductDescription;
                    existingProduct.ProductRating = csproducts.ProductRating;
                    existingProduct.ProductCategoryId = csproducts.ProductCategoryId;
                    existingProduct.ModifiedDate = DateTime.Now;

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                    TempData["toastMsg"] = "Product Updated.";
                }
                catch (Exception ex)
                {
                    TempData["toastMsg"] = $"Exception updating the product : {ex.Message}.";
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var product = _context.CSProducts.Find(id);
            if (product != null)
            {
                string filename = product.ImageUrl;
                _context.CSProducts.Remove(product);
                _context.SaveChanges();
                if (filename != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "product_images", filename);
                    if (System.IO.File.Exists(path))
                    {
                        try
                        {
                            System.IO.File.Delete(path);
                        }
                        catch
                        {
                            TempData["toastErrMsg"] = "Product deletion failed from folder.";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        TempData["toastErrMsg"] = "Image file not found.";
                    }
                }
                else
                {
                    TempData["toastErrMsg"] = "Product not found.";
                }
            }
            TempData["toastMsg"] = "Product Deleted.";

            return RedirectToAction("Index");
        }
    }
}
