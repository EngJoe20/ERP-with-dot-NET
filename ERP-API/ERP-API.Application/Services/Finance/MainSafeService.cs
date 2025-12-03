using ERP_API.Application.DTOs.Finance;
using ERP_API.Application.Interfaces.Finance;
using ERP_API.DataAccess.Entities.Finance;
using ERP_API.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_API.Application.DTOs;
using ERP_API.Application.Interfaces;
using ERP_API.DataAccess.Entities;
using ERP_API.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ERP_API.Application.Services
{
    public class MainSafeService : IMainSafeService
    {

        private readonly IErpUnitOfWork _unitOfWork;

        public MainSafeService(IErpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<MainSafeDto?> GetMainSafeAsync(int id)
        {
            var safe = await _unitOfWork.MainSafes.FindByIdAsync(id);
            if (safe == null) return null;

            return MapToDto(safe);
        }

        public async Task<IEnumerable<MainSafeDto>> GetAllMainSafesAsync()
        {
            var safes = await _unitOfWork.MainSafes.GetAllAsync();
            return safes.Select(MapToDto);
        }

        public async Task<MainSafeDto> CreateMainSafeAsync(CreateMainSafeDto createDto, int userId)
        {
            var safe = new MainSafe
            {
                SafeName = createDto.SafeName,
                OpeningBalance = createDto.OpeningBalance,
                CurrentBalance = createDto.OpeningBalance,
                IsActive = createDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MainSafes.CreateAsync(safe);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(safe);
        }

        public async Task<bool> UpdateMainSafeAsync(int id, UpdateMainSafeDto updateDto)
        {
            var safe = await _unitOfWork.MainSafes.FindByIdAsync(id);
            if (safe == null) return false;

            var oldOpeningBalance = safe.OpeningBalance;

            if (updateDto.SafeName != null)
                safe.SafeName = updateDto.SafeName;

            if (updateDto.OpeningBalance.HasValue)
            {
                var diff = updateDto.OpeningBalance.Value - oldOpeningBalance;
                safe.OpeningBalance = updateDto.OpeningBalance.Value;
                safe.CurrentBalance += diff;
            }

            if (updateDto.IsActive.HasValue)
                safe.IsActive = updateDto.IsActive.Value;

            safe.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.MainSafes.Update(safe);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteMainSafeAsync(int id)
        {
            var result = await _unitOfWork.MainSafes.DeleteAsync(id);
            if (result != null)
            {
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<MainSafeDto> GetOrCreateDefaultSafeAsync()
        {
            var safe = await _unitOfWork.MainSafes.GetAllQueryable()
                .FirstOrDefaultAsync(s => s.Id == 1);

            if (safe == null)
            {
                safe = new MainSafe
                {
                    SafeName = "Main Safe",
                    OpeningBalance = 0,
                    CurrentBalance = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.MainSafes.CreateAsync(safe);
                await _unitOfWork.SaveChangesAsync();
            }

            return MapToDto(safe);
        }

        private MainSafeDto MapToDto(MainSafe safe)
        {
            return new MainSafeDto
            {
                Id = safe.Id,
                SafeName = safe.SafeName,
                OpeningBalance = safe.OpeningBalance,
                CurrentBalance = safe.CurrentBalance,
                IsActive = safe.IsActive,
                CreatedAt = safe.CreatedAt,
                UpdatedAt = safe.UpdatedAt
            };
        }
    }
}
