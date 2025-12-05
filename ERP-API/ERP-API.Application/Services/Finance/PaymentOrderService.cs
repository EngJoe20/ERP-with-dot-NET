using ERP_API.Application.DTOs.Finance;
using ERP_API.Application.Interfaces.Finance;
using ERP_API.DataAccess.Entities.Finance;
using ERP_API.DataAccess.Entities.Customers;
using ERP_API.DataAccess.Entities.Suppliers;
using ERP_API.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP_API.Application.Services.Finance
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
            var entries = await _unitOfWork.PaymentOrder
                .GetAllQueryable()
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
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
                Direction = DTOs.Finance.TransactionDirection.Out,
                CustomerName = e.CustomerTransaction?.Customer?.CustomerName,
                SupplierName = e.SupplierTransaction?.Supplier?.SupplierName,
                PerformedByUserName = e.PerformedByUser?.UserName ?? "Unknown",
                CreatedAt = e.CreatedAt
            });
        }



        public async Task<int> CreatePaymentOrderAsync(CreatePaymentOrderDto createDto, string? userId)
        {
            try
            {
                int referenceId = 0;
                var date = DateTime.UtcNow;

                int? customerTransactionId = null;
                int? supplierTransactionId = null;

                // Validate userId
                if (!string.IsNullOrEmpty(userId))
                {
                    var userExists = await _unitOfWork.UserManager.FindByIdAsync(userId);
                    if (userExists == null)
                        throw new ArgumentException($"User with ID {userId} not found");
                }

                // Verify main safe exists
                var safe = await _unitOfWork.MainSafes.FindByIdAsync(1);
                if (safe == null)
                    throw new InvalidOperationException("Main safe with ID 1 not found!");

                // Create reference record
                switch (createDto.ReferenceTable.ToLower())
                {
                    case "customertransactions":
                        if (!createDto.CustomerId.HasValue)
                            throw new ArgumentException("CustomerId is required");

                        var customer = await _unitOfWork.Customers.FindByIdAsync(createDto.CustomerId.Value);
                        if (customer == null)
                            throw new ArgumentException("Customer not found");

                        var customerTx = new CustomerTransaction
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

                        await _unitOfWork.CustomerTransactions.CreateAsync(customerTx);
                        await _unitOfWork.SaveChangesAsync();

                        customerTransactionId = customerTx.Id;
                        referenceId = customerTx.Id;

                        // Update customer balance (deduct)
                        customer.TotalBalance -= createDto.Amount;
                        customer.UpdatedAt = date;
                        _unitOfWork.Customers.Update(customer);
                        await _unitOfWork.SaveChangesAsync();
                        break;



                    case "suppliertransactions":
                        if (!createDto.SupplierId.HasValue)
                            throw new ArgumentException("SupplierId is required");

                        var supplier = await _unitOfWork.Suppliers.FindByIdAsync(createDto.SupplierId.Value);
                        if (supplier == null)
                            throw new ArgumentException("Supplier not found");

                        var supplierTx = new SupplierTransaction
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

                        await _unitOfWork.SupplierTransactions.CreateAsync(supplierTx);
                        await _unitOfWork.SaveChangesAsync();

                        supplierTransactionId = supplierTx.Id;
                        referenceId = supplierTx.Id;

                        // Update supplier balance (deduct)
                        supplier.TotalBalance -= createDto.Amount;
                        supplier.UpdatedAt = date;
                        _unitOfWork.Suppliers.Update(supplier);
                        await _unitOfWork.SaveChangesAsync();
                        break;



                    case "expenses":
                        var expense = new Expense
                        {
                            ExpenseName = createDto.ExpenseName ?? "Expense",
                            Description = createDto.Description,
                            CreatedAt = date,
                            UpdatedAt = date
                        };

                        await _unitOfWork.Expenses.CreateAsync(expense);
                        await _unitOfWork.SaveChangesAsync();

                        referenceId = expense.Id;
                        break;



                    case "profitsources":
                        var source = new ProfitSource
                        {
                            SourceName = createDto.SourceName ?? "Source",
                            Description = createDto.Description,
                            CreatedAt = date,
                            UpdatedAt = date
                        };

                        await _unitOfWork.ProfitSources.CreateAsync(source);
                        await _unitOfWork.SaveChangesAsync();

                        referenceId = source.Id;
                        break;



                    default:
                        throw new ArgumentException("Invalid reference table");
                }



                // Calculate new balance (deduct)
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
                    ReferenceTable = GetFullName(createDto.ReferenceTable),
                    ReferenceRecordId = referenceId,
                    PerformedByUserId = userId,
                    Direction = DataAccess.Entities.Finance.TransactionDirection.Out,
                    CreatedAt = date,
                    UpdatedAt = date
                };

                await _unitOfWork.MainSafeLedgerEntry.CreateAsync(ledgerEntry);

                // Update safe
                safe.CurrentBalance = newBalance;
                safe.UpdatedAt = date;
                _unitOfWork.MainSafes.Update(safe);

                await _unitOfWork.SaveChangesAsync();



                // Create PaymentOrder row
                var paymentOrder = new PaymentOrder
                {
                    MainSafeId = 1,
                    EntryTimestamp = date,
                    EntryDescription = createDto.Description,
                    DebitAmount = createDto.Amount,
                    BalanceAfterEntry = newBalance,
                    Direction = DataAccess.Entities.Finance.TransactionDirection.Out,
                    CustomerTransactionId = customerTransactionId,
                    SupplierTransactionId = supplierTransactionId,
                    PerformedByUserId = userId,
                    CreatedAt = date,
                    UpdatedAt = date
                };

                await _unitOfWork.PaymentOrder.CreateAsync(paymentOrder);
                await _unitOfWork.SaveChangesAsync();

                return ledgerEntry.Id;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
            }
        }




        private string GetFullName(string reference)
        {
            return reference.ToLower() switch
            {
                "customertransactions" => "CustomerTransactions",
                "suppliertransactions" => "SupplierTransactions",
                "expenses" => "Expenses",
                "profitsources" => "ProfitSources",
                _ => throw new ArgumentException("Invalid reference table")
            };
        }
    }
}
