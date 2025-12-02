using ERP_API.Application.DTOs.User;
using ERP_API.Application.Interfaces.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP_API.API.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto user)
        {

            return Ok(await _accountService.Register(user));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto user)
        {
            var result = await _accountService.LoginAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { result.TokenResult, result.UserName, Claims = result.Claims });
            }
            return Unauthorized(new { user.UserName, Errors = result.Errors.ToArray() });
        }
    }
}
