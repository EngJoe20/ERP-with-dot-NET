using ERP_API.Application.DTOs.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces.Finance
{
    public interface IPaymentOrderService
    {
        Task<IEnumerable<PaymentOrderDto>> GetAllPaymentOrdersAsync();
        Task<int> CreatePaymentOrderAsync(CreatePaymentOrderDto createDto, int userId);
    }
}
