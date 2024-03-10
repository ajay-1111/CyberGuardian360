namespace CyberGuardian360.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CyberGuardian360.DBContext;
    using CyberGuardian360.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class CSProductsController : Controller
    {
        private readonly CyberGuardian360DbContext _context;

        public CSProductsController(CyberGuardian360DbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            this.TempData["NoProducts"] = null;
            var products = this._context.CSProducts.ToList();

            if (products.Count == 0)
            {
                this.TempData["NoProducts"] = "Currently no products available.";
                return this.View();
            }

            var filter = new Filter();
            var productViewModels = products.Select(product => new CSProductsViewModel
            {
                ImageUrl = product.ImageUrl,
                ProductName = product.ProductName,
                ProductCost = product.ProductCost,
                ProductRating = product.ProductRating,
                Id = product.Id,
            }).ToList();

            filter.CSProductsViewModel = productViewModels;

            var list = new List<CheckboxModel>
            {
                new CheckboxModel { Id = 1, Name = "Anti Virus Software", Checked = false },
                new CheckboxModel { Id = 2, Name = "Firewall Solutions", Checked = false },
                new CheckboxModel { Id = 3, Name = "Data Encryption Tools", Checked = false },
            };

            filter.CheckBoxes = list;

            return this.View(filter);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsByFilter(int[]? chkCategories)
        {
            if (chkCategories == null || chkCategories.Length == 0)
            {
                return this.RedirectToAction("Index");
            }

            var products = await this._context.CSProducts
                .Where(p => chkCategories.Contains((int)p.ProductCategoryId))
                .ToListAsync();

            if (products.Count == 0)
            {
                this.TempData["NoProducts"] = "No products available for selected categories.";
                return this.RedirectToAction("Index");
            }

            var productViewModels = products.Select(product => new CSProductsViewModel
            {
                ImageUrl = product.ImageUrl,
                ProductName = product.ProductName,
                ProductCost = product.ProductCost,
                ProductRating = product.ProductRating,
                Id = product.Id,
            }).ToList();

            var filter = new Filter
            {
                CSProductsViewModel = productViewModels,
            };

            var list = new List<CheckboxModel>
            {
                new CheckboxModel { Id = 1, Name = "Anti Virus Software", Checked = false },
                new CheckboxModel { Id = 2, Name = "Firewall Solutions", Checked = false },
                new CheckboxModel { Id = 3, Name = "Data Encryption Tools", Checked = false },
            };

            filter.CheckBoxes = list;

            return this.View("Index", filter);
        }
    }
}
