using ERP_API.Application.DTOs.Sales.SalesReturn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces.Sales
{
    public interface ISalesReturnService
    {
        Task<SalesReturnResponseDto> CreateReturnAsync(CreateSalesReturnDto dto);
        Task<SalesReturnResponseDto?> GetReturnByIdAsync(int id);
        Task<List<SalesReturnListItemDto>> GetAllReturnsAsync();
        Task<List<SalesReturnListItemDto>> GetReturnsByCustomerAsync(int customerId);
        Task<bool> DeleteReturnAsync(int id);
    }
}
