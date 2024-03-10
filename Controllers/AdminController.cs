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
            this._context = context;
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var products = this._context.CSProducts.OrderBy(p => p.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalProducts = this._context.CSProducts.Count();

            var model = new Pagination<CSProducts>(products, totalProducts, page, pageSize);

            return View(model);
        }

        public IActionResult Create()
        {
            this.TempData["AddSuccess"] = null;
            this.TempData["AddError"] = null;
            this.TempData["UpdateSuccess"] = null;
            this.TempData["UpdateError"] = null;
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CSProducts product, IFormFile? ImageUrl)
        {
            this.TempData["AddSuccess"] = null;
            this.TempData["AddError"] = null;

            if (this.ModelState.IsValid)
            {
                try
                {
                    if (ImageUrl is { Length: > 0 })
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageUrl.FileName);

                        var uploadsFolder = Path.Combine(this._webHostEnvironment.WebRootPath, "product_images");

                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        await using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageUrl.CopyToAsync(stream);
                        }

                        product.ImageUrl = uniqueFileName;
                    }

                    product.CreatedDate = DateTime.Now;
                    product.ModifiedDate = DateTime.Now;

                    this._context.CSProducts.Add(product);
                    await this._context.SaveChangesAsync();

                    this.TempData["toastMsg"] = "Product Created.";

                    this.TempData["AddSuccess"] = $"Product {product.Id} is added to Menu.";

                    return this.RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    this.TempData["AddError"] = $"Exception while adding the new Product : {ex.Message}";
                }
            }

            return this.View(product);
        }

        public IActionResult Edit(int id)
        {
            this.TempData["AddSuccess"] = null;
            this.TempData["AddError"] = null;
            this.TempData["UpdateSuccess"] = null;
            this.TempData["UpdateError"] = null;
            var product = this._context.CSProducts.Find(id);
            return this.View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CSProducts product)
        {
            this.TempData["UpdateSuccess"] = null;
            this.TempData["UpdateError"] = null;

            if (id != product.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    this._context.Update(product);
                    await this._context.SaveChangesAsync();
                    this.TempData["toastMsg"] = "Product Updated.";
                    this.TempData["UpdateSuccess"] = $"Product {product.Id} is updated successfully.";
                }
                catch (Exception ex)
                {
                    this.TempData["UpdateError"] = $"Exception in updating the Product : {ex.Message}.";
                }
            }

            return this.RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            this.TempData["AddSuccess"] = null;
            this.TempData["AddError"] = null;
            this.TempData["UpdateSuccess"] = null;
            this.TempData["UpdateError"] = null;

            var product = this._context.CSProducts.Find(id);
            if (product != null)
            {
                this._context.CSProducts.Remove(product);
                this._context.SaveChanges();
                this.TempData["toastMsg"] = "Product Deleted.";
            }

            return this.RedirectToAction("Index");
        }
    }
}
