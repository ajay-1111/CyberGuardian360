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
            // Retrieve all products from the database
            var products = _context.CSProducts.ToList();

            // Check if products list is empty
            if (products.Count == 0)
            {
                // If products list is empty, set an error message using ViewBag
                TempData["NoProducts"] = "Currently no products available.";
                return View();
            }

            // Create a list to hold the view models for all products
            List<CSProductsViewModel> productViewModels = new List<CSProductsViewModel>();

            // Loop through each product and create a view model for it
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

                // Add the view model to the list
                productViewModels.Add(productsModel);
            }

            // Pass the list of view models to the view
            return View(productViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int productid)
        {
            TempData["NoProductFound"] = null;

            // Retrieve the product with the specified id from the database
            var product = await _context.CSProducts.FirstOrDefaultAsync(p => p.Id == productid);

            // Check if product is null
            if (product == null)
            {
                TempData["NoProductFound"] = $"Unable to find the product details for ID : {productid}";
                return View(TempData["NoProductFound"]);
            }

            CSProductsViewModel productmodel = new CSProductsViewModel()
            {
                ImageUrl = product!.ImageUrl,
                ProductName = product.ProductName,
                ProductCost = product.ProductCost,
                ProductRating = product.ProductRating,
                Id = product.Id,
            };

            // Pass the product to the view for rendering
            return View(productmodel);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            if (!string.IsNullOrWhiteSpace(category))
            {
                var categoryEnum = (CSProducts.CSCategories)Enum.Parse(typeof(CSProducts.CSCategories), category, true);

                var products = await _context.CSProducts
                    .Where(p => p.ProductCategoryId == categoryEnum)
                    .ToListAsync();

                if (products.Count == 0)
                {
                    TempData["NoProducts"] = $"No products available for category: {category}";
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

                return View("Index", productviewmodels);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var results = await _context.CSProducts
                .Where(p => p.ProductName.Contains(query))
                .Select(p => new { p.ProductName })
                .ToListAsync();

            return Json(results);
        }
    }
}
