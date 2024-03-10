using CyberGuardian360.DBContext;
using CyberGuardian360.Models.EFDBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CyberGuardian360.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly CyberGuardian360DbContext _context;

        private readonly UserManager<UserRegistration> _userManager;

        private readonly SignInManager<UserRegistration> _signInManager;


        public PurchasesController(CyberGuardian360DbContext context, SignInManager<UserRegistration> signInManager, UserManager<UserRegistration> userManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Buy()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var cartItems = _context.CSUserCartInfo.Where(c => c.UserId == user.Id).ToList();

                    if (cartItems.Any())
                    {
                        // Create an order
                        var order = new UserOrder
                        {
                            UserId = user.Id,
                            OrderDate = DateTime.Now
                        };

                        _context.CSUserOrders.Add(order);
                        await _context.SaveChangesAsync();

                        // Create order items for each cart item
                        foreach (var cartItem in cartItems)
                        {
                            var orderItem = new UserOrderItem
                            {
                                OrderId = order.Id,
                                ProductId = cartItem.ProductId,
                                Quantity = cartItem.Quantity,
                                Price = cartItem.ProductCost
                            };

                            _context.CSUserOrderItems.Add(orderItem);
                        }

                        // Remove the cart items associated with this user
                        _context.CSUserCartInfo.RemoveRange(cartItems);
                        await _context.SaveChangesAsync();
                        TempData["toastMsg"] = "Purchase Completed.";
                        return Json(new { success = true, message = "Purchase Completed." });
                    }

                    return Json(new { success = false, message = "Your cart is empty. Please add items before purchase." });
                }

                return Json(new { success = false, message = "User not available. Please sign in to purchase." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Purchase Failed. Please try again later." });
            }
        }
    }
}
