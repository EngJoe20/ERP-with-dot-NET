using ERP_MVC.Models.Identity;
using ERP_MVC.Models.ViewModels.User;
using ERP_MVC.Services.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_MVC.Controllers.User
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Register Through the API
                var result = await _accountService.Register(model);
                if (result)
                {
                    return RedirectToAction("Index", "Home");
                }


            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> login(UserLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Login Through the API
                var result = await _accountService.Login(model);

                if (result != null)
                {
                    var signinResult = await SigninUser(result);
                }

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }


        private async Task<bool> SigninUser(LoginResult loginResult)
        {
            try
            {
                //List<Claim> claims = new List<Claim>() {
                //    new Claim(ClaimTypes.Name,loginResult.UserName),
                //    new Claim("jwt",loginResult.TokenResult.Token)
                //};

                var claims = loginResult.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();

                claims.Add(new Claim("jwt", loginResult.TokenResult.Token));

                var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, scheme);

                AuthenticationProperties properties = new AuthenticationProperties
                {
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(scheme, new ClaimsPrincipal(claimsIdentity), properties);
                return true;
            }
            catch
            {
                return false;
            }


        }


    }
}
