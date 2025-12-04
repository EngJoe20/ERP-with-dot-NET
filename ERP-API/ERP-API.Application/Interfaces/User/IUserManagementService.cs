using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_API.Application.DTOs.User;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ERP_API.Application.Interfaces.User
{
    public interface IUserManagementService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> UpdateUserAsync(UserUpdateDto user);
    }
}
