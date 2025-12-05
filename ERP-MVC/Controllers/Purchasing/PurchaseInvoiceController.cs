using ERP_MVC.Models.DTOs.Finance;
using ERP_MVC.Models.DTOs.Purchasing;
using ERP_MVC.Services.Purchasing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace ERP_MVC.Controllers
{
    public class PurchaseInvoiceController : Controller
    {
        private readonly PurchaseInvoiceService _purchaseInvoiceService;
        private readonly IHttpClientFactory _httpClientFactory;

        public PurchaseInvoiceController(
            PurchaseInvoiceService purchaseInvoiceService,
            IHttpClientFactory httpClientFactory)
        {
            _purchaseInvoiceService = purchaseInvoiceService;
            _httpClientFactory = httpClientFactory;
        }

        // GET: /PurchaseInvoice/Index
        public async Task<IActionResult> Index()
        {
            var invoices = await _purchaseInvoiceService.GetAllInvoicesAsync();
             return View("~/Views/Purchasing/invoice/Index.cshtml", invoices);
        }

        // GET: /PurchaseInvoice/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();

            var model = new CreatePurchaseInvoiceDto
            {
                InvoiceDate = DateTime.Now,
                Items = new List<PurchaseInvoiceItemDto>()
            };

            return View("~/Views/Purchasing/invoice/Create.cshtml", model);
        }

        // POST: /PurchaseInvoice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePurchaseInvoiceDto model)
        {
            // Remove empty items
            model.Items = model.Items.Where(i => i.ProductPackageId > 0).ToList();

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(model);
            }

            var result = await _purchaseInvoiceService.CreateInvoiceAsync(model);

            if (result)
            {
                TempData["SuccessMessage"] = "Purchase Invoice created successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to create purchase invoice. Please try again.";
            await LoadDropdownsAsync();
            return View("~/Views/Purchasing/invoice/Create.cshtml", model);
        }

        // GET: /PurchaseInvoice/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _purchaseInvoiceService.GetInvoiceByIdAsync(id);

            if (invoice == null)
            {
                TempData["ErrorMessage"] = "Invoice not found.";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Purchasing/invoice/Details.cshtml", invoice);
        }

        // POST: /PurchaseInvoice/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _purchaseInvoiceService.DeleteInvoiceAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Invoice deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete invoice.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper Method
        private async Task LoadDropdownsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7052/api/");

            // Add Token
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
                else
                {
                    ViewBag.SupplierList = new List<SelectListItem>();
                }
            }
            catch
            {
                ViewBag.SupplierList = new List<SelectListItem>();
            }

            // Load Products (ProductPackages)
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
                        Text = $"{p.ProductCode} - {p.ProductName} ({p.PackageTypeName})"
                    }).ToList() ?? new List<SelectListItem>();
                }
                else
                {
                    ViewBag.ProductList = new List<SelectListItem>();
                }
            }
            catch
            {
                ViewBag.ProductList = new List<SelectListItem>();
            }
        }
    }

    // Helper DTOs
    //public class SupplierDto
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //}

    public class ProductPackageDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string PackageTypeName { get; set; } = string.Empty;
    }
}