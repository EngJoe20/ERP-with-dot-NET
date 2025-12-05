using ERP_API.Application.DTOs.Suppliers;
using ERP_API.Application.Interfaces.Suppliers;
using ERP_API.DataAccess.Entities.Suppliers;
using ERP_API.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // ============================================================
        // 1. Get All Suppliers
        // ============================================================
        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();

            return suppliers.Select(s => new SupplierDto
            {
                Id = s.Id,
                SupplierName = s.SupplierName,
                TaxNumber = s.TaxNumber,
                Email = s.Email,
                Phone = s.Phone,
                OpeningBalance = s.OpeningBalance,
                TotalBalance = s.TotalBalance,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });
        }

        // ============================================================
        // 2. Get Supplier By ID
        // ============================================================
        public async Task<SupplierDto?> GetSupplierAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.FindByIdAsync(id);
            if (supplier == null) return null;

            return new SupplierDto
            {
                Id = supplier.Id,
                SupplierName = supplier.SupplierName,
                TaxNumber = supplier.TaxNumber,
                Email = supplier.Email,
                Phone = supplier.Phone,
                OpeningBalance = supplier.OpeningBalance,
                TotalBalance = supplier.TotalBalance,
                Description = supplier.Description,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };
        }

        // ============================================================
        // 3. Get Supplier With Transactions
        // ============================================================
        public async Task<SupplierDetailsDto?> GetSupplierDetailsAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetSupplierWithTransactionsAsync(id);
            if (supplier == null) return null;

            return new SupplierDetailsDto
            {
                Id = supplier.Id,
                SupplierName = supplier.SupplierName,
                TaxNumber = supplier.TaxNumber,
                Email = supplier.Email,
                Phone = supplier.Phone,
                OpeningBalance = supplier.OpeningBalance,
                TotalBalance = supplier.TotalBalance,
                Description = supplier.Description,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt,
                Transactions = supplier.Transactions.Select(t => new SupplierTransactionDto
                {
                    Id = t.Id,
                    TransactionType = t.SupplierTransactionType,
                    TransactionDate = t.TransactionDate,
                    Amount = t.Amount,
                    Direction = t.Direction,
                    Description = t.Description
                }).ToList()
            };
        }

        // ============================================================
        // 4. Get Supplier Transactions Only
        // ============================================================
        public async Task<IEnumerable<SupplierTransactionDto>> GetSupplierTransactionsAsync(int supplierId)
        {
            var supplier = await _unitOfWork.Suppliers.GetSupplierWithTransactionsAsync(supplierId);
            if (supplier == null)
                return Enumerable.Empty<SupplierTransactionDto>();

            return supplier.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new SupplierTransactionDto
                {
                    Id = t.Id,
                    TransactionType = t.SupplierTransactionType,
                    TransactionDate = t.TransactionDate,
                    Amount = t.Amount,
                    Direction = t.Direction,
                    Description = t.Description
                });
        }

        // ============================================================
        // 5. Create Supplier
        // ============================================================
        public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto dto)
        {
            var entity = new Supplier
            {
                SupplierName = dto.SupplierName,
                TaxNumber = dto.TaxNumber,
                Email = dto.Email,
                Phone = dto.Phone,
                OpeningBalance = dto.OpeningBalance,
                TotalBalance = dto.TotalBalance,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Suppliers.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new SupplierDto
            {
                Id = entity.Id,
                SupplierName = entity.SupplierName,
                TaxNumber = entity.TaxNumber,
                Email = entity.Email,
                Phone = entity.Phone,
                OpeningBalance = entity.OpeningBalance,
                TotalBalance = entity.TotalBalance,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        // ============================================================
        // 6. Update Supplier
        // ============================================================
        public async Task<SupplierDto?> UpdateSupplierAsync(int id, UpdateSupplierDto dto)
        {
            var s = await _unitOfWork.Suppliers.FindByIdAsync(id);
            if (s == null) return null;

            s.SupplierName = dto.SupplierName;
            s.TaxNumber = dto.TaxNumber;
            s.Email = dto.Email;
            s.Phone = dto.Phone;
            s.OpeningBalance = dto.OpeningBalance;
            s.TotalBalance = dto.TotalBalance;
            s.Description = dto.Description;
            s.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Suppliers.Update(s);
            await _unitOfWork.SaveChangesAsync();

            return new SupplierDto
            {
                Id = s.Id,
                SupplierName = s.SupplierName,
                TaxNumber = s.TaxNumber,
                Email = s.Email,
                Phone = s.Phone,
                OpeningBalance = s.OpeningBalance,
                TotalBalance = s.TotalBalance,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            };
        }

        // ============================================================
        // 7. Delete Supplier
        // ============================================================
        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var deleted = await _unitOfWork.Suppliers.DeleteAsync(id);
            if (deleted == null) return false;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ============================================================
        // 8. Recalculate Supplier Balance
        // ============================================================
        public async Task RecalculateSupplierBalanceAsync(int supplierId)
        {
            var supplier = await _unitOfWork.Suppliers.GetSupplierWithTransactionsAsync(supplierId);
            if (supplier == null) return;

            decimal balance = supplier.OpeningBalance;

            foreach (var t in supplier.Transactions.OrderBy(t => t.TransactionDate))
            {
                if (t.Direction == SupplierTransactionDirection.In)
                    balance += t.Amount;
                else
                    balance -= t.Amount;
            }

            supplier.TotalBalance = balance;
            supplier.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
