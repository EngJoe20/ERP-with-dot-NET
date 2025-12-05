using ERP_API.Application.DTOs.Finance;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP_API.Application.Interfaces.Finance
{
    public interface IMainSafeLedgerEntryService
    {
        /// <summary>
        /// Get all ledger entries
        /// </summary>
        Task<IEnumerable<MainSafeLedgerEntryDto>> GetAllLedgerEntriesAsync();

        /// <summary>
        /// Get ledger entry by ID
        /// </summary>
        Task<MainSafeLedgerEntryDto?> GetLedgerEntryByIdAsync(int id);

        /// <summary>
        /// Get ledger entry details by ID with full related information
        /// </summary>
        Task<MainSafeLedgerEntryDetailsDto?> GetLedgerEntryDetailsAsync(int id);

        /// <summary>
        /// Get ledger entries by Main Safe ID
        /// </summary>
        Task<IEnumerable<MainSafeLedgerEntryDto>> GetLedgerEntriesByMainSafeIdAsync(int mainSafeId);

        /// <summary>
        /// Get filtered ledger entries
        /// </summary>
        Task<IEnumerable<MainSafeLedgerEntryDto>> GetFilteredLedgerEntriesAsync(MainSafeLedgerEntryFilterDto filter);

        /// <summary>
        /// Get ledger entries by date range
        /// </summary>
        Task<IEnumerable<MainSafeLedgerEntryDto>> GetLedgerEntriesByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get ledger entries by direction (In/Out)
        /// </summary>
        Task<IEnumerable<MainSafeLedgerEntryDto>> GetLedgerEntriesByDirectionAsync(string direction);

        /// <summary>
        /// Get ledger entries by reference table
        /// </summary>
        Task<IEnumerable<MainSafeLedgerEntryDto>> GetLedgerEntriesByReferenceTableAsync(string referenceTable);

        /// <summary>
        /// Create a new ledger entry
        /// </summary>
        Task<int> CreateLedgerEntryAsync(CreateMainSafeLedgerEntryDto createDto, string userId);

        /// <summary>
        /// Update an existing ledger entry
        /// </summary>
        Task UpdateLedgerEntryAsync(int id, UpdateMainSafeLedgerEntryDto updateDto);

        /// <summary>
        /// Delete a ledger entry
        /// </summary>
        Task DeleteLedgerEntryAsync(int id);

        /// <summary>
        /// Get ledger summary for a Main Safe
        /// </summary>
        Task<MainSafeLedgerSummaryDto> GetLedgerSummaryAsync(int mainSafeId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Get latest entries for a Main Safe
        /// </summary>
        Task<IEnumerable<MainSafeLedgerEntryDto>> GetLatestEntriesAsync(int mainSafeId, int count = 10);

        /// <summary>
        /// Check if ledger entry exists
        /// </summary>
        Task<bool> LedgerEntryExistsAsync(int id);
    }
}