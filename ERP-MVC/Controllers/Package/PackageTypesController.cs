using ERP_MVC.Models.DTOs.Inventory.Packages;
using ERP_MVC.Services.Inventory.Package;
using Microsoft.AspNetCore.Mvc;

namespace ERP_MVC.Controllers.Package
{
    public class PackageTypesController : Controller
    {
        private readonly PackageTypeService _packageService;

        public PackageTypesController(PackageTypeService packageService)
        {
            _packageService = packageService;
        }

    
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var packages = await _packageService.GetAllPackageTypes();
            return View(packages);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new PackageTypeInsertDto());
        }


        [HttpPost]
        public async Task<IActionResult> Create(PackageTypeInsertDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            bool success = await _packageService.CreatePackageType(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Package type created successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to create package type. Please try again.");
            return View(dto);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var packageDetails = await _packageService.GetPackageDetailsWithProducts(id);

            if (packageDetails == null)
            {
                return NotFound();
            }

            return View(packageDetails);
        }
    }
}
