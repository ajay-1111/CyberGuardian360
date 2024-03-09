using CyberGuardian360.DBContext;
using CyberGuardian360.Models   ;
using CyberGuardian360.Models.EFDBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberGuardian360.Controllers
{
    public class ProfileController : Controller
    {
        private readonly CyberGuardian360DbContext _dbContext;

        private readonly UserManager<UserRegistration> _userManager;

        private readonly SignInManager<UserRegistration> _signInManager;

        public ProfileController(CyberGuardian360DbContext _dbContext, SignInManager<UserRegistration> signInManager, UserManager<UserRegistration> userManager)
        {
            this._dbContext = _dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }
                var model = new Profile
                {
                    Email = user.Email,
                    GivenName = user.GivenName,
                    Surname = user.Surname,
                    Phone = user.Phone,
                };
                return View(model);
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Profile model)
        {
            if (ModelState.IsValid)
            {
                if (_signInManager.IsSignedIn(User))
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (user == null)
                    {
                        return NotFound();
                    }
                    var userInfo = this._dbContext.AspNetUsers.FirstOrDefault(u => u.Email == user.Email);

                    if (userInfo != null)
                    {
                        userInfo.GivenName = model.GivenName;
                        userInfo.Surname = model.Surname;
                        userInfo.Phone = model.Phone;

                        var result = _dbContext.AspNetUsers.Update(userInfo);
                        await _dbContext.SaveChangesAsync();

                        return RedirectToAction("Index", "CSProducts");
                    }
                }
                ModelState.AddModelError(string.Empty, "User does not exist.");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
