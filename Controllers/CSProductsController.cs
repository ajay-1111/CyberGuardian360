using CyberGuardian360.DBContext;
using CyberGuardian360.Migrations;
using CyberGuardian360.Models;
using CyberGuardian360.Models.EFDBContext;
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

            //List<CheckboxModel> filter
            //if (filter.Where(a => a.Checked).Count() > 0)
            //{
            //    var categories = filter.Where(a => a.Checked).Select(b => b.Id).ToList();
            //    var products = await _context.CSProducts
            //        .Where(s => categories.Contains((int)s.ProductCategoryId))
            //        .ToListAsync();

            //    if (products.Count == 0)
            //    {
            //        TempData["NoProducts"] = $"No products available for category.";
            //        return RedirectToAction("Index");
            //    }

            //    var productviewmodels = products.Select(product => new CSProductsViewModel
            //    {
            //        ImageUrl = product.ImageUrl,
            //        ProductName = product.ProductName,
            //        ProductCost = product.ProductCost,
            //        ProductRating = product.ProductRating,
            //        Id = product.Id
            //    }).ToList();

            //    return View("Index", productviewmodels);
            //}

            return RedirectToAction("Index");
        }

        //[HttpGet]
        //public async Task<IActionResult> GetProductsByCategory(string category)
        //{
        //    if (!string.IsNullOrWhiteSpace(category))
        //    {
        //        var categoryEnum = (CSProducts.CSCategories)Enum.Parse(typeof(CSProducts.CSCategories), category, true);

        //        var products = await _context.CSProducts
        //            .Where(p => p.ProductCategoryId == categoryEnum)
        //            .ToListAsync();

        //        if (products.Count == 0)
        //        {
        //            TempData["NoProducts"] = $"No products available for category: {category}";
        //            return RedirectToAction("Index");
        //        }

        //        var productviewmodels = products.Select(product => new CSProductsViewModel
        //        {
        //            ImageUrl = product.ImageUrl,
        //            ProductName = product.ProductName,
        //            ProductCost = product.ProductCost,
        //            ProductRating = product.ProductRating,
        //            Id = product.Id
        //        }).ToList();

        //        return View("Index", productviewmodels);
        //    }

        //    return RedirectToAction("Index");
        //}
    }
}
