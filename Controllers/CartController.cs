using CyberGuardian360.DBContext;
using CyberGuardian360.Models;
using CyberGuardian360.Models.EFDBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace CyberGuardian360.Controllers
{
    public class CartController : Controller
    {
        private readonly CyberGuardian360DbContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly UserManager<UserRegistration> _userManager;

        private readonly SignInManager<UserRegistration> _signInManager;

        public CartController(CyberGuardian360DbContext context, IHttpContextAccessor httpContextAccessor, UserManager<UserRegistration> userManager, SignInManager<UserRegistration> signInManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartItems = await GetCartItemsForCurrentUser();
            ViewBag.CartItemCount = await GetCartItemCount();
            return View(cartItems);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(int id)
        {
            //TempData["CartItemCount"] = null;

            var product = await _context.CSProducts.FirstOrDefaultAsync(p => p.Id == id);

            var checkIfUserSignedInOrNot = _signInManager.IsSignedIn(User);

            if (checkIfUserSignedInOrNot)
            {
                var user = _userManager.GetUserId(User);

                if (user != null)
                {
                    var getTheQuantity = await _context.CSUserCartInfo.FirstOrDefaultAsync(p => p.ProductId == id);
                    if (getTheQuantity != null)
                    {
                        getTheQuantity.Quantity += 1;
                        _context.Update(getTheQuantity);
                    }
                    else
                    {
                        if (product != null)
                        {
                            UserCartInfo newUserCartInfo = new UserCartInfo()
                            {
                                ProductId = product.Id,
                                UserId = user,
                                Quantity = 1,
                                ProductCost = product.ProductCost
                            };
                            await _context.CSUserCartInfo.AddAsync(newUserCartInfo);
                        }
                    }
                }
                else
                {
                    UserCartInfo newUserCartInfo = new UserCartInfo()
                    {
                        ProductId = product!.Id,
                        UserId = user!,
                        Quantity = 1,
                        ProductCost = product.ProductCost
                    };

                    await _context.CSUserCartInfo.AddAsync(newUserCartInfo);
                }
                await _context.SaveChangesAsync();
            }


            // Set the cart item count in TempData
            int cartItemCount = await GetCartItemCount();
            //TempData["CartItemCount"] = cartItemCount;
            HttpContext.Session.SetString("CartItemCount", cartItemCount.ToString());


            return RedirectToAction("Index", "CSProducts");
        }

        [HttpPost]
        public async Task<IActionResult> IncrementCart(int id)
        {
            //TempData["CartItemCount"] = null;

            var product = await _context.CSProducts.FirstOrDefaultAsync(p => p.Id == id);

            var checkIfUserSignedInOrNot = _signInManager.IsSignedIn(User);

            if (checkIfUserSignedInOrNot)
            {
                var user = _userManager.GetUserId(User);

                if (user != null)
                {
                    var getTheQuantity = await _context.CSUserCartInfo.FirstOrDefaultAsync(p => p.ProductId == id);
                    if (getTheQuantity != null)
                    {
                        getTheQuantity.Quantity += 1;
                        _context.Update(getTheQuantity);
                    }
                    else
                    {
                        if (product != null)
                        {
                            UserCartInfo newUserCartInfo = new UserCartInfo()
                            {
                                ProductId = product.Id,
                                UserId = user,
                                Quantity = 1,
                                ProductCost = product.ProductCost
                            };
                            await _context.CSUserCartInfo.AddAsync(newUserCartInfo);
                        }
                    }
                }
                else
                {
                    UserCartInfo newUserCartInfo = new UserCartInfo()
                    {
                        ProductId = product!.Id,
                        UserId = user!,
                        Quantity = 1,
                        ProductCost = product.ProductCost
                    };

                    await _context.CSUserCartInfo.AddAsync(newUserCartInfo);
                }
                await _context.SaveChangesAsync();
            }


            // Set the cart item count in TempData
            int cartItemCount = await GetCartItemCount();
            //TempData["CartItemCount"] = cartItemCount;
            HttpContext.Session.SetString("CartItemCount", cartItemCount.ToString());

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> DecrementCart(int id)
        {
            //TempData["CartItemCount"] = null;

            var product = await _context.CSProducts.FirstOrDefaultAsync(p => p.Id == id);

            var checkIfUserSignedInOrNot = _signInManager.IsSignedIn(User);

            if (checkIfUserSignedInOrNot)
            {
                var user = _userManager.GetUserId(User);

                if (user != null)
                {
                    var getTheQuantity = await _context.CSUserCartInfo.FirstOrDefaultAsync(p => p.ProductId == id);
                    if (getTheQuantity != null)
                    {
                        getTheQuantity.Quantity -= 1;
                        _context.Update(getTheQuantity);
                    }
                    else
                    {
                        if (product != null)
                        {
                            UserCartInfo newUserCartInfo = new UserCartInfo()
                            {
                                ProductId = product.Id,
                                UserId = user,
                                Quantity = 1,
                                ProductCost = product.ProductCost
                            };
                            await _context.CSUserCartInfo.AddAsync(newUserCartInfo);
                        }
                    }
                }
                else
                {
                    UserCartInfo newUserCartInfo = new UserCartInfo()
                    {
                        ProductId = product!.Id,
                        UserId = user!,
                        Quantity = 1,
                        ProductCost = product.ProductCost
                    };

                    await _context.CSUserCartInfo.AddAsync(newUserCartInfo);
                }
                await _context.SaveChangesAsync();
            }


            // Set the cart item count in TempData
            int cartItemCount = await GetCartItemCount();
            //TempData["CartItemCount"] = cartItemCount;
            HttpContext.Session.SetString("CartItemCount", cartItemCount.ToString());

            return RedirectToAction("Index", "Cart");
        }

        private async Task<int> GetCartItemCount()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    return await _context.CSUserCartInfo
                        .Where(u => u.UserId == user.Id)
                        .SumAsync(u => u.Quantity);
                }
            }
            return 0;
        }
        private async Task<List<UserCartViewModel>> GetCartItemsForCurrentUser()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var cartItems = await (from uc in _context.CSUserCartInfo
                                           join p in _context.CSProducts on uc.ProductId equals p.Id
                                           where uc.UserId == user.Id
                                           select new UserCartViewModel
                                           {
                                               ProductId = p.Id,
                                               ImageUrl = p.ImageUrl,
                                               ProductName = p.ProductName,
                                               ProductCost = p.ProductCost,
                                               Quantity = uc.Quantity
                                           }).ToListAsync();

                    return cartItems;
                }
            }
            return new List<UserCartViewModel>();
        }

        [HttpPost]
        public async Task<IActionResult> Clear(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var cartItem = await _context.CSUserCartInfo.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);

            if (cartItem != null)
            {
                _context.CSUserCartInfo.Remove(cartItem);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
