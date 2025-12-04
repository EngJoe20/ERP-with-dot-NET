using ERP_API.Application.DTOs.Suppliers;
using ERP_API.Application.Interfaces.Suppliers;
using ERP_API.DataAccess.Entities.Suppliers;
using ERP_API.DataAccess.Interfaces;

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

            if (supplier == null)
                return null;

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

        public async Task<SupplierDetailsDto?> GetSupplierDetailsAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers
                .GetSupplierWithTransactionsAsync(id);

            if (supplier == null) return null;

            return new SupplierDetailsDto
            {
                Id = supplier.Id,
                SupplierName = supplier.SupplierName,
                TotalBalance = supplier.TotalBalance,
                Description = supplier.Description,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt,
                Transactions = supplier.Transactions.Select(t => new SupplierTransactionDto
                {
                    Id = t.Id,
                    TransactionType = t.TransactionType,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    Direction = t.Direction,
                    Description = t.Description
                }).ToList()
            };
        }

        public async Task<IEnumerable<SupplierTransactionDto>> GetSupplierTransactionsAsync(int supplierId)
        {
            var supplier = await _unitOfWork.Suppliers
                .GetSupplierWithTransactionsAsync(supplierId);

            if (supplier == null)
                return Enumerable.Empty<SupplierTransactionDto>();

            return supplier.Transactions.Select(t => new SupplierTransactionDto
            {
                Id = t.Id,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                TransactionDate = t.TransactionDate,
                Direction = t.Direction,
                Description = t.Description
            });
        }

        public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto dto)
        {
            var entity = new Supplier
            {
                SupplierName = dto.SupplierName,
                TotalBalance = dto.TotalBalance,
                Description = dto.Description
            };

            await _unitOfWork.Suppliers.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new SupplierDto
            {
                Id = entity.Id,
                SupplierName = entity.SupplierName,
                TotalBalance = entity.TotalBalance,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public async Task<SupplierDto?> UpdateSupplierAsync(int id, UpdateSupplierDto dto)
        {
            var supplier = await _unitOfWork.Suppliers.FindByIdAsync(id);
            if (supplier == null)
                return null;

            supplier.SupplierName = dto.SupplierName;
            supplier.TotalBalance = dto.TotalBalance;
            supplier.Description = dto.Description;
            supplier.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

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

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var deleted = await _unitOfWork.Suppliers.DeleteAsync(id);
            if (deleted == null)
                return false;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
