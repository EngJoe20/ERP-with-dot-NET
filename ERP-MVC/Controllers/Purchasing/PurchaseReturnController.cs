using ERP_MVC.Models.DTOs.Finance;
using ERP_MVC.Models.DTOs.Purchasing;
using ERP_MVC.Services.Purchasing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace ERP_MVC.Controllers
{
    public class PurchaseReturnController : Controller
    {
        private readonly PurchaseReturnService _purchaseReturnService;
        private readonly IHttpClientFactory _httpClientFactory;

        public PurchaseReturnController(
            PurchaseReturnService purchaseReturnService,
            IHttpClientFactory httpClientFactory)
        {
            _purchaseReturnService = purchaseReturnService;
            _httpClientFactory = httpClientFactory;
        }

        // GET: /PurchaseReturn/Index
        public async Task<IActionResult> Index()
        {
            var returns = await _purchaseReturnService.GetAllReturnsAsync();
            return View("~/Views/Purchasing/return/Index.cshtml", returns);
        }

        // GET: /PurchaseReturn/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();

            var model = new CreatePurchaseReturnDto
            {
                ReturnDate = DateTime.Now,
                Items = new List<PurchaseReturnItemDto>()
            };

            return View("~/Views/Purchasing/invoice/Create.cshtml", model);
        }

        // POST: /PurchaseReturn/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePurchaseReturnDto model)
        {
            // Remove empty items
            model.Items = model.Items.Where(i => i.ProductPackageId > 0).ToList();

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(model);
            }

            var result = await _purchaseReturnService.CreateReturnAsync(model);

            if (result)
            {
                TempData["SuccessMessage"] = "Purchase Return created successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to create purchase return. Please try again.";
            await LoadDropdownsAsync();
            return View("~/Views/Purchasing/invoice/Create.cshtml", model);
        }

        // GET: /PurchaseReturn/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var returnData = await _purchaseReturnService.GetReturnByIdAsync(id);

            if (returnData == null)
            {
                TempData["ErrorMessage"] = "Return not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(returnData);
        }

        // POST: /PurchaseReturn/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _purchaseReturnService.DeleteReturnAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Return deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete return.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper Method
        private async Task LoadDropdownsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7052/api/");

            var token = User.FindFirst("jwt")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            // Load Suppliers
            try
            {
                var suppliersResponse = await client.GetAsync("Suppliers");
                if (suppliersResponse.IsSuccessStatusCode)
                {
                    var json = await suppliersResponse.Content.ReadAsStringAsync();
                    var suppliers = JsonSerializer.Deserialize<List<SupplierDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    ViewBag.SupplierList = suppliers?.Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.SupplierName
                    }).ToList() ?? new List<SelectListItem>();
                }
            }
            catch
            {
                ViewBag.SupplierList = new List<SelectListItem>();
            }

            // Load Products
            try
            {
                var productsResponse = await client.GetAsync("ProductPackages");
                if (productsResponse.IsSuccessStatusCode)
                {
                    var json = await productsResponse.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<ProductPackageDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    ViewBag.ProductList = products?.Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = $"{p.ProductCode} - {p.ProductName}"
                    }).ToList() ?? new List<SelectListItem>();
                }
            }
            catch
            {
                ViewBag.ProductList = new List<SelectListItem>();
            }
        }
    }
}