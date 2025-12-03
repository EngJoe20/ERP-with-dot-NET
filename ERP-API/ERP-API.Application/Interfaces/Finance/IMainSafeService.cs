using ERP_API.Application.DTOs.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces.Finance
{
    public interface IMainSafeService
    {
        Task<MainSafeDto?> GetMainSafeAsync(int id);
        Task<IEnumerable<MainSafeDto>> GetAllMainSafesAsync();
        Task<MainSafeDto> CreateMainSafeAsync(CreateMainSafeDto createDto, int userId);
        Task<bool> UpdateMainSafeAsync(int id, UpdateMainSafeDto updateDto);
        Task<bool> DeleteMainSafeAsync(int id);
        Task<MainSafeDto> GetOrCreateDefaultSafeAsync();
    }
}
