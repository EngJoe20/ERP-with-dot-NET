using ERP_API.Application.DTOs.Finance;
using ERP_API.Application.Interfaces;
using ERP_API.Application.Interfaces.Finance;
using ERP_API.DataAccess.Entities;
using ERP_API.DataAccess.Entities.Customers;
using ERP_API.DataAccess.Entities.Finance;
using ERP_API.DataAccess.Entities.Suppliers;
using ERP_API.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ERP_API.Application.Services
{
    public class PaymentOrderService : IPaymentOrderService
    {
        private readonly IErpUnitOfWork _unitOfWork;

        public PaymentOrderService(IErpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PaymentOrderDto>> GetAllPaymentOrdersAsync()
        {
            var entries = await _unitOfWork.MainSafeLedgerEntry
                .GetAllQueryable()
                .Include(e => e.PerformedByUser)
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
                .Where(e => e.Direction == "out")
                .OrderBy(e => e.EntryTimestamp)
                .ToListAsync();

            return entries.Select(e => new PaymentOrderDto
            {
                Id = e.Id,
                MainSafeId = e.MainSafeId,
                EntryTimestamp = e.EntryTimestamp,
                EntryDescription = e.EntryDescription,
                DebitAmount = e.DebitAmount,
                BalanceAfterEntry = e.BalanceAfterEntry,
                Direction = e.Direction,
                CustomerName = e.CustomerTransaction?.Customer?.CustomerName,
                SupplierName = e.SupplierTransaction?.Supplier?.SupplierName,
                PerformedByUserName = e.PerformedByUser?.UserName ?? "Unknown",
                CreatedAt = e.CreatedAt
            });
        }

        public async Task<int> CreatePaymentOrderAsync(CreatePaymentOrderDto createDto, int userId)
        {
            int referenceId = 0;
            var date = DateTime.UtcNow;

            // Create reference record based on type
            switch (createDto.ReferenceTable.ToLower())
            {
                case "customertransactions":
                    if (!createDto.CustomerId.HasValue)
                        throw new ArgumentException("CustomerId is required for customer transactions");

                    var customerTransaction = new CustomerTransaction
                    {
                        CustomerId = createDto.CustomerId.Value,
                        CustomerTransactionType = CustomerTransactionType.Payment,
                        TransactionDate = date,
                        Amount = createDto.Amount,
                        Direction = CustomerTransactionDirection.Out,
                        Description = createDto.Description,
                        CreatedAt = date,
                        UpdatedAt = date
                    };

                    await _unitOfWork.CustomerTransactions.CreateAsync(customerTransaction);
                    await _unitOfWork.SaveChangesAsync();
                    referenceId = customerTransaction.Id;

                    // Update customer balance
                    var customer = await _unitOfWork.Customers.FindByIdAsync(createDto.CustomerId.Value);
                    if (customer != null)
                    {
                        customer.TotalBalance -= createDto.Amount;
                        customer.UpdatedAt = date;
                        _unitOfWork.Customers.Update(customer);
                    }
                    break;

                case "suppliertransactions":
                    if (!createDto.SupplierId.HasValue)
                        throw new ArgumentException("SupplierId is required for supplier transactions");

                    var supplierTransaction = new SupplierTransaction
                    {
                        SupplierId = createDto.SupplierId.Value,
                        SupplierTransactionType = SupplierTransactionType.Payment,
                        TransactionDate = date,
                        Amount = createDto.Amount,
                        Direction = SupplierTransactionDirection.Out,
                        Description = createDto.Description,
                        CreatedAt = date,
                        UpdatedAt = date
                    };

                    await _unitOfWork.SupplierTransactions.CreateAsync(supplierTransaction);
                    await _unitOfWork.SaveChangesAsync();
                    referenceId = supplierTransaction.Id;

                    // Update supplier balance
                    var supplier = await _unitOfWork.Suppliers.FindByIdAsync(createDto.SupplierId.Value);
                    if (supplier != null)
                    {
                        supplier.TotalBalance -= createDto.Amount;
                        supplier.UpdatedAt = date;
                        _unitOfWork.Suppliers.Update(supplier);
                    }
                    break;

                case "expenses":
                    var expense = new Expense
                    {
                        ExpenseName = createDto.ExpenseName ?? "Unnamed Expense",
                        Description = createDto.Description,
                        CreatedAt = date,
                        UpdatedAt = date
                    };

                    await _unitOfWork.Expenses.CreateAsync(expense);
                    await _unitOfWork.SaveChangesAsync();
                    referenceId = expense.Id;
                    break;

                case "profitsources":
                    var profitSource = new ProfitSource
                    {
                        SourceName = createDto.SourceName ?? "Unnamed Source",
                        Description = createDto.Description,
                        CreatedAt = date,
                        UpdatedAt = date
                    };

                    await _unitOfWork.ProfitSources.CreateAsync(profitSource);
                    await _unitOfWork.SaveChangesAsync();
                    referenceId = profitSource.Id;
                    break;

                default:
                    throw new ArgumentException("Invalid reference table");
            }

            // Update main safe balance
            var safe = await _unitOfWork.MainSafes.FindByIdAsync(1);
            if (safe == null)
                throw new InvalidOperationException("Main safe not found");

            var newBalance = safe.CurrentBalance - createDto.Amount;

            // Create ledger entry
            var ledgerEntry = new MainSafeLedgerEntry
            {
                MainSafeId = 1,
                EntryTimestamp = date,
                EntryDescription = createDto.Description,
                DebitAmount = createDto.Amount,
                CreditAmount = 0,
                BalanceAfterEntry = newBalance,
                ReferenceTable = GetFullReferenceTableName(createDto.ReferenceTable),
                ReferenceRecordId = referenceId,
                PerformedByUserId = userId.ToString(),
                Direction = "out",
                CreatedAt = date,
                UpdatedAt = date
            };

            await _unitOfWork.MainSafeLedgerEntry.CreateAsync(ledgerEntry);

            // Update safe balance
            safe.CurrentBalance = newBalance;
            safe.UpdatedAt = date;
            _unitOfWork.MainSafes.Update(safe);

            await _unitOfWork.SaveChangesAsync();

            return ledgerEntry.Id;
        }

        private string GetFullReferenceTableName(string referenceTable)
        {
            return referenceTable.ToLower() switch
            {
                "customertransactions" => "ERP_API.DataAccess.Entities.CustomerTransaction",
                "suppliertransactions" => "ERP_API.DataAccess.Entities.SupplierTransaction",
                "expenses" => "ERP_API.DataAccess.Entities.Expense",
                "profitsources" => "ERP_API.DataAccess.Entities.ProfitSource",
                _ => throw new ArgumentException("Invalid reference table")
            };
        }
    }
}
