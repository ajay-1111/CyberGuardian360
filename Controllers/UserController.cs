using CyberGuardian360.DBContext;
using CyberGuardian360.Models;
using CyberGuardian360.Models.EFDBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
                    Password = model.Password,
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
                    return RedirectToAction("Index", "CSProducts");
                }

                ModelState.AddModelError(string.Empty, "Email or Password is incorrect.");
            }

            return View(model);
        }
    }
}
