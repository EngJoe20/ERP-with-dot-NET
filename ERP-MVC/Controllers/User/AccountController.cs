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

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.Register(model);
                if (result) return RedirectToAction("Privacy", "Home");
            }
            return View(model);
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> login(UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _accountService.Login(model);

                if (result != null)
                {
                    var signinResult = await SigninUser(result);
                    if (signinResult)
                        return RedirectToAction("Privacy", "Home");
                }
            }
            catch (Exception ex)
            {
                // الرسالة القادمة من الـ API تظهر مباشرة
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task<bool> SigninUser(LoginResult loginResult)
        {
            try
            {
                var claims = loginResult.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
                claims.Add(new Claim("jwt", loginResult.TokenResult.Token));

                var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
                var claimsIdentity = new ClaimsIdentity(claims, scheme);

                var properties = new AuthenticationProperties { IsPersistent = true };
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
