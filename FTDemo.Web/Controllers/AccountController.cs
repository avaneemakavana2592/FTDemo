using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FTDemo.Common;
using FTDemo.Database.Entities;
using FTDemo.Web.Models.User;

namespace FTDemo.Web.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, true, false);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
                else
                {
                    ViewBag.Error = "Invalid credential entered.";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [Route("Register")]
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser appUser = new AppUser
                    {
                        Email = registerModel.Email,
                        UserName = registerModel.Email,
                        FirstName = registerModel.FirstName,
                        LastName = registerModel.LastName,
                    };

                    var result = await userManager.CreateAsync(appUser, registerModel.Password);

                    if (result.Succeeded)
                    {
                        var addRoleResult = await userManager.AddToRoleAsync(appUser, Constants.Role.ContentView);
                        if (addRoleResult.Succeeded)
                            registerModel.IsSuccess = true;
                        else
                            ViewBag.Error = GetIdentityErrors(addRoleResult.Errors);
                    }
                    else
                        ViewBag.Error = GetIdentityErrors(result.Errors);
                }
                else
                    return StatusCode((int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return View(registerModel);
        }

        private string GetIdentityErrors(IEnumerable<IdentityError> errors)
        {
            string errorMsg = string.Empty;
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    errorMsg += $"{error.Code}; {error.Description};\n";
                }
            }

            return errorMsg;
        }
    }
}