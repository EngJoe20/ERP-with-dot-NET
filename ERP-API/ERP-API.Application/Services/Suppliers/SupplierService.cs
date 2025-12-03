using ERP_API.Application.DTOs.Suppliers;
using ERP_API.Application.Interfaces.Suppliers;
using ERP_API.DataAccess.Entities.Suppliers;
using ERP_API.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.Services.Suppliers
{
    public class SupplierService : ISupplierService
    {
        private readonly IErpUnitOfWork _unitOfWork;

        public SupplierService(IErpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            return suppliers.Select(s => new SupplierDto
            {
                Id = s.Id,
                SupplierName = s.SupplierName,
                TotalBalance = s.TotalBalance,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });
        }

        public async Task<SupplierDto?> GetSupplierAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.FindByIdAsync(id);
            if (supplier == null) return null;

            return new SupplierDto
            {
                Id = supplier.Id,
                SupplierName = supplier.SupplierName,
                TotalBalance = supplier.TotalBalance,
                Description = supplier.Description,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };
        }
    }
}
