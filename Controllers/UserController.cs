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
                    TempData["toastMsg"] = "Sign Up Completed.";
                    return RedirectToAction("SignIn", "User");
                }
                else
                {
                    foreach (var error in signup.Errors)
                    {
                        errorMessage += error.Description;
                    }
                    TempData["toastErrMsg"] = errorMessage;
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
                            HttpContext.Session.SetString("IsAdmin", "1");
                        }
                        else
                        {
                            HttpContext.Session.SetString("IsAdmin", "0");
                        }
                    }
                    return RedirectToAction("Index", "CSProducts");
                }
                TempData["toastErrMsg"] = "Invalid Credentials.";
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            var user = await this.userManager.GetUserAsync(User);

            HttpContext.Session.Clear();

            await this.signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
