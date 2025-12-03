using ERP_API.Application.DTOs.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces.User
{
    public interface IAccountService
    {
        Task<IdentityResult> Register(UserRegisterDto user);

        Task<UserLoginResult> LoginAsync(UserLoginDto user);
    }
}
