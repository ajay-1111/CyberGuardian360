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
                    // Check if a file is uploaded
                    if (ImageUrl is { Length: > 0 })
                    {
                        // Generate a unique filename for the image
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageUrl.FileName);

                        // Get the path of the wwwroot/img folder where images will be stored
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "product_images");

                        // Combine the unique filename with the path to store the image
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Copy the uploaded file to the specified path
                        await using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageUrl.CopyToAsync(stream);
                        }

                        // Update the ImageUrl property of the product with the new filename
                        product.ImageUrl = uniqueFileName;
                    }

                    // Set CreateDateTime and ModifieDateTime
                    product.CreatedDate = DateTime.Now;
                    product.ModifiedDate = DateTime.Now;

                    // Add the product to the database
                    _context.CSProducts.Add(product);
                    await _context.SaveChangesAsync();

                    TempData["AddSuccess"] = $"Product {product.Id} is added to Menu.";

                    // Redirect to the product list page after successful creation
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log or handle the exception appropriately
                    TempData["AddError"] = $"Exception while adding the new Product : {ex.Message}";
                }
            }

            // If the model state is not valid, return the view with the model data and errors
            return View(product);
        }


        // Action method to display form for updating product details
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
        public async Task<IActionResult> Edit(int id, CSProducts product)
        {
            TempData["UpdateSuccess"] = null;
            TempData["UpdateError"] = null;

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update other properties of the product as usual
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["UpdateSuccess"] = $"Product {product.Id} is updated successfully.";
                }
                catch (Exception ex)
                {
                    TempData["UpdateError"] = $"Exception in updating the Product : {ex.Message}.";
                }
            }
            return RedirectToAction("Index");
        }


        // Action method to delete a product
        public IActionResult Delete(int id)
        {
            TempData["AddSuccess"] = null;
            TempData["AddError"] = null;
            TempData["UpdateSuccess"] = null;
            TempData["UpdateError"] = null;

            var product = _context.CSProducts.Find(id);
            if (product != null) _context.CSProducts.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
