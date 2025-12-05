using ERP_API.Application.DTOs.Customers;
using ERP_API.Application.DTOs.Finance;
using ERP_API.Application.DTOs.Suppliers;
using ERP_API.Application.Interfaces.Customers;
using ERP_API.Application.Interfaces.Finance;

using ERP_API.Application.Interfaces.Suppliers;
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

namespace ERP_API.Application.Services.Finance
{
    public class ReceiptOrderService : IReceiptOrderService
    {
        private readonly IErpUnitOfWork _unitOfWork;

        public ReceiptOrderService(IErpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ReceiptOrderDto>> GetAllReceiptOrdersAsync()
        {
            var entries = await _unitOfWork.ReceiptOrder
                .GetAllQueryable()
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
                .OrderBy(e => e.EntryTimestamp)
                .ToListAsync();

            return entries.Select(e => new ReceiptOrderDto
            {
                Id = e.Id,
                MainSafeId = e.MainSafeId,
                EntryTimestamp = e.EntryTimestamp,
                EntryDescription = e.EntryDescription,
                CreditAmount = e.CreditAmount,
                BalanceAfterEntry = e.BalanceAfterEntry,
                Direction = (DTOs.Finance.TransactionDirection)e.Direction,
                CustomerName = e.CustomerTransaction?.Customer?.CustomerName,
                SupplierName = e.SupplierTransaction?.Supplier?.SupplierName,
                PerformedByUserName = e.PerformedByUser?.UserName ?? "Unknown",
                CreatedAt = e.CreatedAt
            });
        }



        public async Task<int> CreateReceiptOrderAsync(CreateReceiptOrderDto createDto, string? userId)
        {
            try
            {
                int referenceId = 0;
                var date = DateTime.UtcNow;

                // Track which transaction type was created
                int? customerTransactionId = null;
                int? supplierTransactionId = null;

                // Validate userId exists if provided
                if (!string.IsNullOrEmpty(userId))
                {
                    var userExists = await _unitOfWork.UserManager.FindByIdAsync(userId);
                    if (userExists == null)
                    {
                        throw new ArgumentException($"User with ID {userId} not found");
                    }
                }

                // Verify MainSafe exists FIRST
                var safe = await _unitOfWork.MainSafes.FindByIdAsync(1);
                if (safe == null)
                    throw new InvalidOperationException("Main safe with ID 1 not found. Please create it first.");

                // Create reference record based on type
                switch (createDto.ReferenceTable.ToLower())
                {
                    case "customertransactions":
                        if (!createDto.CustomerId.HasValue)
                            throw new ArgumentException("CustomerId is required for customer transactions");

                        var customer = await _unitOfWork.Customers.FindByIdAsync(createDto.CustomerId.Value);
                        if (customer == null)
                            throw new ArgumentException($"Customer with ID {createDto.CustomerId.Value} not found");

                        var customerTransaction = new CustomerTransaction
                        {
                            CustomerId = createDto.CustomerId.Value,
                            CustomerTransactionType = CustomerTransactionType.Receipt,
                            TransactionDate = date,
                            Amount = createDto.Amount,
                            Direction = CustomerTransactionDirection.In,
                            Description = createDto.Description,
                            CreatedAt = date,
                            UpdatedAt = date
                        };

                        await _unitOfWork.CustomerTransactions.CreateAsync(customerTransaction);
                        await _unitOfWork.SaveChangesAsync();

                        // Verify the transaction was saved and has an ID
                        if (customerTransaction.Id == 0)
                            throw new InvalidOperationException("Failed to create customer transaction");

                        referenceId = customerTransaction.Id;
                        customerTransactionId = customerTransaction.Id;

                        // Update customer balance
                        customer.TotalBalance += createDto.Amount;
                        customer.UpdatedAt = date;
                        _unitOfWork.Customers.Update(customer);
                        await _unitOfWork.SaveChangesAsync();
                        break;

                    case "suppliertransactions":
                        if (!createDto.SupplierId.HasValue)
                            throw new ArgumentException("SupplierId is required for supplier transactions");

                        var supplier = await _unitOfWork.Suppliers.FindByIdAsync(createDto.SupplierId.Value);
                        if (supplier == null)
                            throw new ArgumentException($"Supplier with ID {createDto.SupplierId.Value} not found");

                        var supplierTransaction = new SupplierTransaction
                        {
                            SupplierId = createDto.SupplierId.Value,
                            SupplierTransactionType = SupplierTransactionType.Receipt,
                            TransactionDate = date,
                            Amount = createDto.Amount,
                            Direction = SupplierTransactionDirection.In,
                            Description = createDto.Description,
                            CreatedAt = date,
                            UpdatedAt = date
                        };

                        await _unitOfWork.SupplierTransactions.CreateAsync(supplierTransaction);
                        await _unitOfWork.SaveChangesAsync();

                        // Verify the transaction was saved and has an ID
                        if (supplierTransaction.Id == 0)
                            throw new InvalidOperationException("Failed to create supplier transaction");

                        referenceId = supplierTransaction.Id;
                        supplierTransactionId = supplierTransaction.Id;

                        // Update supplier balance
                        supplier.TotalBalance += createDto.Amount;
                        supplier.UpdatedAt = date;
                        _unitOfWork.Suppliers.Update(supplier);
                        await _unitOfWork.SaveChangesAsync();
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

                        if (expense.Id == 0)
                            throw new InvalidOperationException("Failed to create expense");

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

                        if (profitSource.Id == 0)
                            throw new InvalidOperationException("Failed to create profit source");

                        referenceId = profitSource.Id;
                        break;

                    default:
                        throw new ArgumentException("Invalid reference table");
                }

                // Calculate new balance
                var newBalance = safe.CurrentBalance + createDto.Amount;

                // Create ledger entry
                var ledgerEntry = new MainSafeLedgerEntry
                {
                    MainSafeId = 1,
                    EntryTimestamp = date,
                    EntryDescription = createDto.Description,
                    DebitAmount = 0,
                    CreditAmount = createDto.Amount,
                    BalanceAfterEntry = newBalance,
                    ReferenceTable = GetFullReferenceTableName(createDto.ReferenceTable),
                    ReferenceRecordId = referenceId,
                    PerformedByUserId = userId,
                    Direction = (DataAccess.Entities.Finance.TransactionDirection)DTOs.Finance.TransactionDirection.In,
                    CreatedAt = date,
                    UpdatedAt = date
                };

                await _unitOfWork.MainSafeLedgerEntry.CreateAsync(ledgerEntry);

                // Update safe balance
                safe.CurrentBalance = newBalance;
                safe.UpdatedAt = date;
                _unitOfWork.MainSafes.Update(safe);

                await _unitOfWork.SaveChangesAsync();

                // Add to ReceiptOrders table
                var receiptOrder = new ReceiptOrder
                {
                    MainSafeId = 1,
                    EntryTimestamp = date,
                    EntryDescription = createDto.Description,
                    CreditAmount = createDto.Amount,
                    BalanceAfterEntry = newBalance,
                    Direction = (DataAccess.Entities.Finance.TransactionDirection)DTOs.Finance.TransactionDirection.In,
                    CustomerTransactionId = customerTransactionId,
                    SupplierTransactionId = supplierTransactionId,
                    PerformedByUserId = userId,
                    CreatedAt = date,
                    UpdatedAt = date
                };

                await _unitOfWork.ReceiptOrder.CreateAsync(receiptOrder);
                await _unitOfWork.SaveChangesAsync();

                return ledgerEntry.Id;
            }
            catch (DbUpdateException ex)
            {
                // Log detailed error information
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                throw new InvalidOperationException($"Database error: {innerMessage}", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetFullReferenceTableName(string referenceTable)
        {
            return referenceTable.ToLower() switch
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




   