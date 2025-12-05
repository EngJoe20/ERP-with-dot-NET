using ERP_API.Application.DTOs.Finance;
using ERP_API.Application.Interfaces.Finance;
using ERP_API.DataAccess.Entities.Finance;
using ERP_API.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionDirection = ERP_API.Application.DTOs.Finance.TransactionDirection;

namespace ERP_API.Application.Services.Finance
{
    public class MainSafeLedgerEntryService : IMainSafeLedgerEntryService
    {
        private readonly IErpUnitOfWork _unitOfWork;

        public MainSafeLedgerEntryService(IErpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MainSafeLedgerEntryDto>> GetAllLedgerEntriesAsync()
        {
            var entries = await _unitOfWork.MainSafeLedgerEntry
                .GetAllQueryable()
                .Include(e => e.PerformedByUser)
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
                .Include(e => e.Expense)
                .Include(e => e.ProfitSource)
                .OrderByDescending(e => e.EntryTimestamp)
                .ToListAsync();

            return MapToDto(entries);
        }

        public async Task<MainSafeLedgerEntryDto?> GetLedgerEntryByIdAsync(int id)
        {
            var entry = await _unitOfWork.MainSafeLedgerEntry
                .GetAllQueryable()
                .Include(e => e.PerformedByUser)
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
                .Include(e => e.Expense)
                .Include(e => e.ProfitSource)
                .FirstOrDefaultAsync(e => e.Id == id);

            return entry == null ? null : MapToDto(entry);
        }

        public async Task<MainSafeLedgerEntryDetailsDto?> GetLedgerEntryDetailsAsync(int id)
        {
            var entry = await _unitOfWork.MainSafeLedgerEntry
                .GetAllQueryable()
                .Include(e => e.MainSafe)
                .Include(e => e.PerformedByUser)
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
                .Include(e => e.Expense)
                .Include(e => e.ProfitSource)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entry == null) return null;

            return new MainSafeLedgerEntryDetailsDto
            {
                Id = entry.Id,
                MainSafeId = entry.MainSafeId,
                MainSafeName = entry.MainSafe?.SafeName ?? "N/A",
                EntryTimestamp = entry.EntryTimestamp,
                EntryDescription = entry.EntryDescription,
                DebitAmount = entry.DebitAmount,
                CreditAmount = entry.CreditAmount,
                BalanceAfterEntry = entry.BalanceAfterEntry,
                ReferenceTable = entry.ReferenceTable,
                ReferenceRecordId = entry.ReferenceRecordId,
                Direction = entry.Direction.ToString(),
                PerformedByUserId = entry.PerformedByUserId,
                PerformedByUserName = entry.PerformedByUser?.UserName ?? "Unknown",
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt,
                CustomerTransaction = entry.CustomerTransaction != null ? new CustomerTransactionInfo
                {
                    Id = entry.CustomerTransaction.Id,
                    CustomerName = entry.CustomerTransaction.Customer?.CustomerName ?? "N/A",
                    Amount = entry.CustomerTransaction.Amount,
                    TransactionType = entry.CustomerTransaction.CustomerTransactionType.ToString()
                } : null,
                SupplierTransaction = entry.SupplierTransaction != null ? new SupplierTransactionInfo
                {
                    Id = entry.SupplierTransaction.Id,
                    SupplierName = entry.SupplierTransaction.Supplier?.SupplierName ?? "N/A",
                    Amount = entry.SupplierTransaction.Amount,
                    TransactionType = entry.SupplierTransaction.SupplierTransactionType.ToString()
                } : null,
                Expense = entry.Expense != null ? new ExpenseInfo
                {
                    Id = entry.Expense.Id,
                    ExpenseName = entry.Expense.ExpenseName,
                    Description = entry.Expense.Description
                } : null,
                ProfitSource = entry.ProfitSource != null ? new ProfitSourceInfo
                {
                    Id = entry.ProfitSource.Id,
                    SourceName = entry.ProfitSource.SourceName,
                    Description = entry.ProfitSource.Description
                } : null
            };
        }

        public async Task<IEnumerable<MainSafeLedgerEntryDto>> GetLedgerEntriesByMainSafeIdAsync(int mainSafeId)
        {
            var entries = await _unitOfWork.MainSafeLedgerEntry
                .GetAllQueryable()
                .Where(e => e.MainSafeId == mainSafeId)
                .Include(e => e.PerformedByUser)
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
                .Include(e => e.Expense)
                .Include(e => e.ProfitSource)
                .OrderByDescending(e => e.EntryTimestamp)
                .ToListAsync();

            return MapToDto(entries);
        }

        public async Task<IEnumerable<MainSafeLedgerEntryDto>> GetFilteredLedgerEntriesAsync(MainSafeLedgerEntryFilterDto filter)
        {
            var query = _unitOfWork.MainSafeLedgerEntry.GetAllQueryable();

            if (filter.MainSafeId.HasValue)
                query = query.Where(e => e.MainSafeId == filter.MainSafeId.Value);

            if (filter.StartDate.HasValue)
                query = query.Where(e => e.EntryTimestamp >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(e => e.EntryTimestamp <= filter.EndDate.Value);

            if (!string.IsNullOrEmpty(filter.Direction))
            {
              
                if (Enum.TryParse<TransactionDirection>(filter.Direction, true, out var direction))
                    query = query.Where(e => (int)e.Direction == (int)direction);
            }

            if (!string.IsNullOrEmpty(filter.ReferenceTable))
                query = query.Where(e => e.ReferenceTable.Contains(filter.ReferenceTable));

            if (filter.MinAmount.HasValue)
                query = query.Where(e => (e.DebitAmount + e.CreditAmount) >= filter.MinAmount.Value);

            if (filter.MaxAmount.HasValue)
                query = query.Where(e => (e.DebitAmount + e.CreditAmount) <= filter.MaxAmount.Value);

            var entries = await query
                .Include(e => e.PerformedByUser)
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
                .Include(e => e.Expense)
                .Include(e => e.ProfitSource)
                .OrderByDescending(e => e.EntryTimestamp)
                .ToListAsync();

            return MapToDto(entries);
        }

        public async Task<IEnumerable<MainSafeLedgerEntryDto>> GetLedgerEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var entries = await _unitOfWork.MainSafeLedgerEntry
                .GetAllQueryable()
                .Where(e => e.EntryTimestamp >= startDate && e.EntryTimestamp <= endDate)
                .Include(e => e.PerformedByUser)
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
                .Include(e => e.Expense)
                .Include(e => e.ProfitSource)
                .OrderByDescending(e => e.EntryTimestamp)
                .ToListAsync();

            return MapToDto(entries);
        }

        public async Task<IEnumerable<MainSafeLedgerEntryDto>> GetLedgerEntriesByDirectionAsync(string direction)
        {
            if (!Enum.TryParse<TransactionDirection>(direction, true, out var transactionDirection))
                return Enumerable.Empty<MainSafeLedgerEntryDto>();

            var entries = await _unitOfWork.MainSafeLedgerEntry
                .GetAllQueryable()
                .Where(e => (int)e.Direction == (int)transactionDirection)
                .Include(e => e.PerformedByUser)
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
                .Include(e => e.Expense)
                .Include(e => e.ProfitSource)
                .OrderByDescending(e => e.EntryTimestamp)
                .ToListAsync();

            return MapToDto(entries);
        }

        public async Task<IEnumerable<MainSafeLedgerEntryDto>> GetLedgerEntriesByReferenceTableAsync(string referenceTable)
        {
            var entries = await _unitOfWork.MainSafeLedgerEntry
                .GetAllQueryable()
                .Where(e => e.ReferenceTable.Contains(referenceTable))
                .Include(e => e.PerformedByUser)
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
                .Include(e => e.Expense)
                .Include(e => e.ProfitSource)
                .OrderByDescending(e => e.EntryTimestamp)
                .ToListAsync();

            return MapToDto(entries);
        }

        public async Task<int> CreateLedgerEntryAsync(CreateMainSafeLedgerEntryDto createDto, string userId)
        {
            if (!Enum.TryParse<TransactionDirection>(createDto.Direction, true, out var direction))
                throw new ArgumentException("Invalid direction value");

            // Get current balance
            var safe = await _unitOfWork.MainSafes.FindByIdAsync(createDto.MainSafeId);
            if (safe == null)
                throw new InvalidOperationException($"Main safe with ID {createDto.MainSafeId} not found");

            // Calculate new balance
            var balanceChange = createDto.CreditAmount - createDto.DebitAmount;
            var newBalance = safe.CurrentBalance + balanceChange;

            var entry = new MainSafeLedgerEntry
            {
                MainSafeId = createDto.MainSafeId,
                EntryTimestamp = DateTime.UtcNow,
                EntryDescription = createDto.EntryDescription,
                DebitAmount = createDto.DebitAmount,
                CreditAmount = createDto.CreditAmount,
                BalanceAfterEntry = newBalance,
                ReferenceTable = createDto.ReferenceTable,
                ReferenceRecordId = createDto.ReferenceRecordId,
                PerformedByUserId = userId,
                Direction = (DataAccess.Entities.Finance.TransactionDirection)direction,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MainSafeLedgerEntry.CreateAsync(entry);

            // Update main safe balance
            safe.CurrentBalance = newBalance;
            safe.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MainSafes.Update(safe);

            await _unitOfWork.SaveChangesAsync();

            return entry.Id;
        }

        public async Task UpdateLedgerEntryAsync(int id, UpdateMainSafeLedgerEntryDto updateDto)
        {
            var entry = await _unitOfWork.MainSafeLedgerEntry.FindByIdAsync(id);
            if (entry == null)
                throw new InvalidOperationException($"Ledger entry with ID {id} not found");

            if (!Enum.TryParse<TransactionDirection>(updateDto.Direction, true, out var direction))
                throw new ArgumentException("Invalid direction value");

            // Calculate balance adjustment
            var oldBalanceChange = entry.CreditAmount - entry.DebitAmount;
            var newBalanceChange = updateDto.CreditAmount - updateDto.DebitAmount;
            var balanceAdjustment = newBalanceChange - oldBalanceChange;

            // Update entry
            entry.EntryDescription = updateDto.EntryDescription;
            entry.DebitAmount = updateDto.DebitAmount;
            entry.CreditAmount = updateDto.CreditAmount;
            entry.Direction = (DataAccess.Entities.Finance.TransactionDirection)direction;
            entry.UpdatedAt = DateTime.UtcNow;

            // Recalculate balance
            entry.BalanceAfterEntry += balanceAdjustment;

            _unitOfWork.MainSafeLedgerEntry.Update(entry);

            // Update main safe balance
            var safe = await _unitOfWork.MainSafes.FindByIdAsync(entry.MainSafeId);
            if (safe != null)
            {
                safe.CurrentBalance += balanceAdjustment;
                safe.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.MainSafes.Update(safe);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteLedgerEntryAsync(int id)
        {
            var entry = await _unitOfWork.MainSafeLedgerEntry.FindByIdAsync(id);
            if (entry == null)
                throw new InvalidOperationException($"Ledger entry with ID {id} not found");
            var balanceChange = entry.CreditAmount - entry.DebitAmount;
            await _unitOfWork.MainSafeLedgerEntry.DeleteAsync(id);

            var safe = await _unitOfWork.MainSafes.FindByIdAsync(entry.MainSafeId);
            if (safe != null)
            {
                safe.CurrentBalance -= balanceChange;
                safe.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.MainSafes.Update(safe);
            }

            await _unitOfWork.SaveChangesAsync();
        }


        public async Task<MainSafeLedgerSummaryDto> GetLedgerSummaryAsync(int mainSafeId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _unitOfWork.MainSafeLedgerEntry
                .GetAllQueryable()
                .Where(e => e.MainSafeId == mainSafeId);

            if (startDate.HasValue)
                query = query.Where(e => e.EntryTimestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.EntryTimestamp <= endDate.Value);

            var entries = await query.ToListAsync();

            var safe = await _unitOfWork.MainSafes.FindByIdAsync(mainSafeId);

            return new MainSafeLedgerSummaryDto
            {
                TotalEntries = entries.Count,
                TotalDebits = entries.Sum(e => e.DebitAmount),
                TotalCredits = entries.Sum(e => e.CreditAmount),
                CurrentBalance = safe?.CurrentBalance ?? 0,
                OpeningBalance = entries.FirstOrDefault()?.BalanceAfterEntry ?? 0,
                ClosingBalance = entries.LastOrDefault()?.BalanceAfterEntry ?? 0,
                PeriodStart = startDate,
                PeriodEnd = endDate
            };
        }

        public async Task<IEnumerable<MainSafeLedgerEntryDto>> GetLatestEntriesAsync(int mainSafeId, int count = 10)
        {
            var entries = await _unitOfWork.MainSafeLedgerEntry
                .GetAllQueryable()
                .Where(e => e.MainSafeId == mainSafeId)
                .Include(e => e.PerformedByUser)
                .Include(e => e.CustomerTransaction)
                    .ThenInclude(t => t!.Customer)
                .Include(e => e.SupplierTransaction)
                    .ThenInclude(t => t!.Supplier)
                .Include(e => e.Expense)
                .Include(e => e.ProfitSource)
                .OrderByDescending(e => e.EntryTimestamp)
                .Take(count)
                .ToListAsync();

            return MapToDto(entries);
        }

        public async Task<bool> LedgerEntryExistsAsync(int id)
        {
            return await _unitOfWork.MainSafeLedgerEntry
                .GetAllQueryable()
                .AnyAsync(e => e.Id == id);
        }

        // Helper methods
        private IEnumerable<MainSafeLedgerEntryDto> MapToDto(IEnumerable<MainSafeLedgerEntry> entries)
        {
            return entries.Select(MapToDto);
        }

        private MainSafeLedgerEntryDto MapToDto(MainSafeLedgerEntry entry)
        {
            return new MainSafeLedgerEntryDto
            {
                Id = entry.Id,
                MainSafeId = entry.MainSafeId,
                EntryTimestamp = entry.EntryTimestamp,
                EntryDescription = entry.EntryDescription,
                DebitAmount = entry.DebitAmount,
                CreditAmount = entry.CreditAmount,
                BalanceAfterEntry = entry.BalanceAfterEntry,
                ReferenceTable = entry.ReferenceTable,
                ReferenceRecordId = entry.ReferenceRecordId,
                Direction = (TransactionDirection)entry.Direction,
                PerformedByUserId = entry.PerformedByUserId,
                PerformedByUserName = entry.PerformedByUser?.UserName ?? "Unknown",
                CreatedAt = entry.CreatedAt,
                CustomerName = entry.CustomerTransaction?.Customer?.CustomerName,
                SupplierName = entry.SupplierTransaction?.Supplier?.SupplierName,
                ExpenseName = entry.Expense?.ExpenseName,
                ProfitSourceName = entry.ProfitSource?.SourceName
            };
        }
    }
}