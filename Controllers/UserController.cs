using CyberGuardian360.DBContext;
using CyberGuardian360.Models;
using CyberGuardian360.Models.EFDBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberGuardian360.Controllers
{
    public class UserController : Controller
    {
        private readonly CyberGuardian360DbContext dbContext;
        private readonly UserManager<UserRegistration> userManager;
        private readonly SignInManager<UserRegistration> signInManager;

        public UserController(CyberGuardian360DbContext dbContext, SignInManager<UserRegistration> signInManager, UserManager<UserRegistration> userManager)
        {
            this.dbContext = dbContext;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View(new SignInViewModel());
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            TempData["SignUpMessage"] = "";
            TempData["SignUpErrorMessage"] = "";

            if (ModelState.IsValid)
            {
                var errorMessage = string.Empty;

                var newUser = new UserRegistration()
                {
                    UserName = model.Email,
                    GivenName = model.GivenName,
                    Surname = model.Surname,
                    Email = model.Email,
                    Phone = model.Phone,
                };

                var signup = await userManager.CreateAsync(newUser, model.Password);

                if (signup.Succeeded)
                {
                    await signInManager.SignInAsync(newUser, isPersistent: false);
                    TempData["SignUpMessage"] = "Registration successful!";
                    return RedirectToAction("SignIn", "User");
                }
                else
                {
                    foreach (var error in signup.Errors)
                    {
                        errorMessage += error.Description;
                    }

                    TempData["SignUpErrorMessage"] = errorMessage;
                    return RedirectToAction("SignUp", "User");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = dbContext.Users.Where(e => e.Email == model.Email).FirstOrDefault();

                    if (user != null)
                    {
                        if (user.IsAdmin)
                        {
                            //TempData["IsAdmin"] = true;
                            HttpContext.Session.SetString("IsAdmin", "1");
                        }
                        else
                        {
                            HttpContext.Session.SetString("IsAdmin", "0");
                            //TempData["IsAdmin"] = false;
                        }
                    }
                    return RedirectToAction("Index", "CSProducts");
                }
                TempData["LoginError"] = "Incorrect username/password. Register here if you are new.";

            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            var user = await this.userManager.GetUserAsync(User);

            HttpContext.Session.Clear();

            await this.signInManager.SignOutAsync();

            

            //if (user != null)
            //{
            //    var cartItems = await this.dbContext.CSUserCartInfo
            //        .Where(u => u.UserId == user.Id)
            //        .ToListAsync();

            //    if (cartItems.Any())
            //    {
            //        this.dbContext.CSUserCartInfo.RemoveRange(cartItems);
            //        await this.dbContext.SaveChangesAsync();
            //    }
            //}

            return RedirectToAction("Index", "Home");
        }
    }
}
