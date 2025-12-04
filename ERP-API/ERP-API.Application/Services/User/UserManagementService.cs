using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_API.Application.DTOs.User;
using ERP_API.Application.Interfaces.User;
using ERP_API.DataAccess.Entities.User;
using ERP_API.DataAccess.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP_API.Application.Services.User
{
    internal class UserManagementService : IUserManagementService
    {
        private readonly IErpUnitOfWork _uow;

        public UserManagementService(IErpUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _uow.UserManager.Users.ToListAsync();
            var list = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _uow.UserManager.GetRolesAsync(user);
                list.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BirthDate = user.BirthDate,
                    Roles = roles.ToArray()
                });
            }
            return list;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _uow.UserManager.FindByIdAsync(id);
            if (user == null) return null;

            var roles = await _uow.UserManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Roles = roles.ToArray()
            };
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _uow.UserManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _uow.UserManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(UserUpdateDto userDto)
        {
            var user = await _uow.UserManager.FindByIdAsync(userDto.Id);
            if (user == null) return false;

            if (!string.IsNullOrEmpty(userDto.Email)) user.Email = userDto.Email;
            if (!string.IsNullOrEmpty(userDto.UserName)) user.UserName = userDto.UserName;
            if (!string.IsNullOrEmpty(userDto.FirstName)) user.FirstName = userDto.FirstName;
            if (!string.IsNullOrEmpty(userDto.LastName)) user.LastName = userDto.LastName;
            if (userDto.BirthDate.HasValue) user.BirthDate = userDto.BirthDate;

            var result = await _uow.UserManager.UpdateAsync(user);
            if (!result.Succeeded) return false;

            if (userDto.Roles != null)
            {
                var currentRoles = await _uow.UserManager.GetRolesAsync(user);
                var rolesToAdd = userDto.Roles.Except(currentRoles);
                var rolesToRemove = currentRoles.Except(userDto.Roles);

                await _uow.UserManager.AddToRolesAsync(user, rolesToAdd);
                await _uow.UserManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            return true;
        }
    }
}
