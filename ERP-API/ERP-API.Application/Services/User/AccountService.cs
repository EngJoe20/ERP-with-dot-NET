using ERP_API.Application.DTOs.User;
using ERP_API.Application.Interfaces.User;
using ERP_API.DataAccess.Entities.User;
using ERP_API.DataAccess.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Services.User
{
    internal class AccountService : IAccountService
    {
        private readonly IErpUnitOfWork _uow;

        public AccountService(IErpUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IdentityResult> Register(UserRegisterDto user)
        {
            AppUser identityUser = new AppUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,

                Email = user.Email,
                UserName = user.UserName,
            };

            var result = await _uow.UserManager.CreateAsync(identityUser, user.Password);
            if (result.Succeeded)
            {
                if (user.Roles != null && user.Roles.Any())
                {
                    foreach (var role in user.Roles)
                    {
                        result = await _uow.UserManager.AddToRoleAsync(identityUser, role);
                        if (!result.Succeeded)
                        {
                            throw new Exception($"Error add user {user.UserName} to role {role}.");
                        }
                    }
                }
                else
                {
                    result = await _uow.UserManager.AddToRoleAsync(identityUser, "users");
                }
            }
            return result;


        }

        public async Task<UserLoginResult> LoginAsync(UserLoginDto user)
        {
            var identityUser = await _uow.UserManager.FindByNameAsync(user.UserName);
            var loginResult = new UserLoginResult();
            if (identityUser == null)
            {
                loginResult.Errors.Add("Invalid User Name!.");
                return loginResult;
            }
            var result = await _uow.UserManager.CheckPasswordAsync(identityUser, user.Password);
            if (result == false)
            {
                loginResult.Errors.Add("Invalid Password!.");
                return loginResult;
            }

            var token = await _uow.TokenManager.GetTokenAsync(identityUser);

            var refreshToken = _uow.TokenManager.GetRefreshToken();

            identityUser.RefreshToken = refreshToken.Token;
            identityUser.RefreshTokenExpiryTime = refreshToken.TokenExpiryTime;

            await _uow.UserManager.UpdateAsync(identityUser);

            loginResult.UserName = identityUser.UserName ?? string.Empty;
            loginResult.Claims = token.Claims;

            loginResult.TokenResult.Token = token.Token;
            loginResult.TokenResult.TokenExpiryTime = token.TokenExpiryTime;

            loginResult.TokenResult.RefreshToken = refreshToken.Token;
            loginResult.TokenResult.RefreshTokenExpiryTime = refreshToken.TokenExpiryTime;



            loginResult.Succeeded = true;
            return loginResult;
        }

    }
}
