using CyberGuardian360.DBContext;
using CyberGuardian360.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberGuardian360.Controllers
{
    public class CSProductsController : Controller
    {
        private readonly CyberGuardian360DbContext _context;
        public CSProductsController(CyberGuardian360DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            TempData["NoProducts"] = null;
            var products = _context.CSProducts.ToList();

            if (products.Count == 0)
            {
                TempData["NoProducts"] = "Currently no products available.";
                return View();
            }

            Filter filter = new Filter();
            List<CSProductsViewModel> productViewModels = new List<CSProductsViewModel>();

            foreach (var product in products)
            {
                CSProductsViewModel productsModel = new CSProductsViewModel()
                {
                    ImageUrl = product.ImageUrl,
                    ProductName = product.ProductName,
                    ProductCost = product.ProductCost,
                    ProductRating = product.ProductRating,
                    ProductDescription = product.ProductDescription,
                    Id = product.Id,
                };

                productViewModels.Add(productsModel);
            }
            filter.CSProductsViewModel = productViewModels;
            var list = new List<CheckboxModel>
            {
                new CheckboxModel{Id = 1, Name = "Anti Virus Software", Checked = false},
                new CheckboxModel{Id = 2, Name = "Firewall Solutions", Checked = false},
                new CheckboxModel{Id = 3, Name = "Data Encryption Tools", Checked = false}
            };
            filter.CheckBoxes = list;

            return View(filter);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsByFilter(int[] chkCategories)
        {

            if (chkCategories.Length > 0)
            {
                var products = await _context.CSProducts
                    .Where(s => chkCategories.Contains((int)s.ProductCategoryId)).ToListAsync();

                if (products.Count == 0)
                {
                    TempData["NoProducts"] = $"No products available for category.";
                    return RedirectToAction("Index");
                }

                var productviewmodels = products.Select(product => new CSProductsViewModel
                {
                    ImageUrl = product.ImageUrl,
                    ProductName = product.ProductName,
                    ProductCost = product.ProductCost,
                    ProductRating = product.ProductRating,
                    Id = product.Id
                }).ToList();

                Filter filter = new Filter();
                filter.CSProductsViewModel = productviewmodels;
                var list = new List<CheckboxModel>
                {
                    new CheckboxModel{Id = 1, Name = "Anti Virus Software"},
                    new CheckboxModel{Id = 2, Name = "Firewall Solutions"},
                    new CheckboxModel{Id = 3, Name = "Data Encryption Tools"}
                };

                foreach (var item in list)
                {
                    if (chkCategories.Contains(item.Id))
                    {
                        item.Checked = true;
                    }
                    else
                    {
                        item.Checked = false;
                    }
                }
                filter.CheckBoxes = list;

                return PartialView("ProductListPartialView", filter);
            }

            return RedirectToAction("Index");
        }
    }
}
