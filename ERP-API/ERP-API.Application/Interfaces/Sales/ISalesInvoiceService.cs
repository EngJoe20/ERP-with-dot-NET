using ERP_API.Application.DTOs.Sales.SalesInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces.Sales
{
    public interface ISalesInvoiceService
    {
        Task<SalesInvoiceResponseDto> CreateInvoiceAsync(CreateSalesInvoiceDto dto);
        Task<SalesInvoiceResponseDto?> GetInvoiceByIdAsync(int id);
        Task<List<SalesInvoiceListItemDto>> GetAllInvoicesAsync();
        Task<List<SalesInvoiceListItemDto>> GetInvoicesByCustomerAsync(int customerId);
        Task<bool> DeleteInvoiceAsync(int id);
    }
}
