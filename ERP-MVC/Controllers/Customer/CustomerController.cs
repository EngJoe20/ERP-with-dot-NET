using ERP_MVC.Models.DTOs.Customers;
using ERP_MVC.Services.Customers;
using Microsoft.AspNetCore.Mvc;

namespace ERP_MVC.Controllers.Customers
{
    public class CustomerController : Controller
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: Customer List
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return View(customers);
        }

        // GET: Add Customer Form
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCustomerDto());
        }

        // POST: Create Customer
        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            bool success = await _customerService.CreateCustomerAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Customer added successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error saving customer. Please try again.");
            return View(dto);
        }

        // GET: Customer Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerService.GetCustomerDetailsAsync(id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // GET: Edit Customer Form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
                return NotFound();

            var updateDto = new UpdateCustomerDto
            {
                CustomerName = customer.CustomerName,
                TaxNumber = customer.TaxNumber,
                Email = customer.Email,
                Phone = customer.Phone,
                OpeningBalance = customer.OpeningBalance,
                TotalBalance = customer.TotalBalance,
                Description = customer.Description
            };

            ViewBag.CustomerId = id;
            return View(updateDto);
        }

        // POST: Update Customer
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCustomerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            bool success = await _customerService.UpdateCustomerAsync(dto.Id, dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Customer updated successfully!";
                return RedirectToAction("Details", new { id = dto.Id });
            }

            ModelState.AddModelError("", "Error updating customer. Please try again.");
            return View(dto);
        }

        // POST: Delete Customer
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await _customerService.DeleteCustomerAsync(id);

            if (success)
                TempData["SuccessMessage"] = "Customer deleted successfully!";
            else
                TempData["ErrorMessage"] = "Error deleting customer.";

            return RedirectToAction("Index");
        }

        // Optional: AJAX endpoint to fetch transactions (if needed)
        [HttpGet]
        public async Task<IActionResult> Transactions(int customerId)
        {
            var transactions = await _customerService.GetCustomerTransactionsAsync(customerId);
            return Json(transactions);
        }
    }
}
