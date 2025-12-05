using ERP_MVC.Models.DTOs.Finance;
using ERP_MVC.Services.Finance;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ERP_MVC.Controllers
{
    public class MainSafeController : Controller
    {
        private readonly MainSafeService _mainSafeService;
        private readonly MainSafeLedgerEntryService _ledgerEntryService;

        public MainSafeController(
            MainSafeService mainSafeService,
            MainSafeLedgerEntryService ledgerEntryService)
        {
            _mainSafeService = mainSafeService;
            _ledgerEntryService = ledgerEntryService;
        }

        // GET: MainSafe/Statement
        public async Task<IActionResult> Statement(int? mainSafeId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Get the main safe (default or specific)
                MainSafeDto mainSafe;
                if (mainSafeId.HasValue)
                {
                    mainSafe = await _mainSafeService.GetMainSafeByIdAsync(mainSafeId.Value);
                }
                else
                {
                    mainSafe = await _mainSafeService.GetStatementAsync();
                }

                if (mainSafe == null)
                {
                    TempData["Error"] = "Main safe not found";
                    return RedirectToAction("Index");
                }

                // Get ledger entries
                var entries = await _ledgerEntryService.GetLedgerEntriesByMainSafeIdAsync(mainSafe.Id);

                // Apply date filtering if provided
                if (startDate.HasValue || endDate.HasValue)
                {
                    var filter = new MainSafeLedgerEntryFilterDto
                    {
                        MainSafeId = mainSafe.Id,
                        StartDate = startDate,
                        EndDate = endDate
                    };
                    entries = await _ledgerEntryService.GetFilteredLedgerEntriesAsync(filter);
                }

                // Get summary
                var summary = await _ledgerEntryService.GetLedgerSummaryAsync(mainSafe.Id, startDate, endDate);

                ViewBag.MainSafe = mainSafe;
                ViewBag.Summary = summary;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                return View(entries);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading statement: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // GET: MainSafe/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var safes = await _mainSafeService.GetAllMainSafesAsync();
                return View(safes);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading main safes: {ex.Message}";
                return View();
            }
        }

        // GET: MainSafe/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var safe = await _mainSafeService.GetMainSafeByIdAsync(id);
                if (safe == null)
                {
                    TempData["Error"] = "Main safe not found";
                    return RedirectToAction("Index");
                }

                // Get latest entries
                var entries = await _ledgerEntryService.GetLatestEntriesAsync(id, 20);
                ViewBag.LatestEntries = entries;

                return View(safe);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading details: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // GET: MainSafe/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MainSafe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMainSafeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                var success = await _mainSafeService.CreateMainSafeAsync(dto);
                if (success)
                {
                    TempData["Success"] = "Main safe created successfully";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = "Failed to create main safe";
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating main safe: {ex.Message}";
                return View(dto);
            }
        }

        // GET: MainSafe/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var safe = await _mainSafeService.GetMainSafeByIdAsync(id);
                if (safe == null)
                {
                    TempData["Error"] = "Main safe not found";
                    return RedirectToAction("Index");
                }

                var updateDto = new UpdateMainSafeDto
                {
                    SafeName = safe.SafeName,
                    OpeningBalance = safe.OpeningBalance,
                    IsActive = safe.IsActive
                };

                ViewBag.SafeId = id;
                return View(updateDto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading main safe: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: MainSafe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateMainSafeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.SafeId = id;
                    return View(dto);
                }

                var success = await _mainSafeService.UpdateMainSafeAsync(id, dto);
                if (success)
                {
                    TempData["Success"] = "Main safe updated successfully";
                    return RedirectToAction("Details", new { id });
                }

                TempData["Error"] = "Failed to update main safe";
                ViewBag.SafeId = id;
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating main safe: {ex.Message}";
                ViewBag.SafeId = id;
                return View(dto);
            }
        }

        // GET: MainSafe/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var safe = await _mainSafeService.GetMainSafeByIdAsync(id);
                if (safe == null)
                {
                    TempData["Error"] = "Main safe not found";
                    return RedirectToAction("Index");
                }

                return View(safe);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading main safe: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: MainSafe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _mainSafeService.DeleteMainSafeAsync(id);
                if (success)
                {
                    TempData["Success"] = "Main safe deleted successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to delete main safe";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting main safe: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // GET: MainSafe/EntryDetails/5
        public async Task<IActionResult> EntryDetails(int id)
        {
            try
            {
                var entry = await _ledgerEntryService.GetLedgerEntryDetailsAsync(id);
                if (entry == null)
                {
                    TempData["Error"] = "Ledger entry not found";
                    return RedirectToAction("Statement");
                }

                return View(entry);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading entry details: {ex.Message}";
                return RedirectToAction("Statement");
            }
        }
    }
}