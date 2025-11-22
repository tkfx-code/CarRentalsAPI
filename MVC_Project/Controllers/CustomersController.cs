using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Models;
using MVC_Project.Services;
using CarRentalsClassLibrary.Model;
using System.Security.Claims;

namespace MVC_Project.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerClientService _customerService;
        private readonly IBookingClientService _bookingService;

        public CustomersController(ICustomerClientService customerService, IBookingClientService bookingService)
        {
            _customerService = customerService;
            _bookingService = bookingService;
        }


        // GET: Customers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return View(customers);
        }

        // GET: Customers/Details/5
        [Authorize]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerDetailsAsync(id);


            if (customer == null)
            {
                return NotFound();
            }
            var customerViewModel = customer.Data;
            return View(customerViewModel);
        }

        [Authorize]
        // GET: Customers/Profile
        //See current users profile
        public async Task<IActionResult> Profile()
        {
            //Use .Name since ASPNETCORE uses email as log in credentials
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (User.IsInRole("Admin") || User.IsInRole("SuperUser"))
            {
                // If the user is an admin, redirect to the admin index
                return RedirectToAction("Admin", "Home");
            }

            var customer = await _customerService.GetCustomerDetailsAsync(currentUserId);

            if (customer == null)
            {
                // For debugging: show message
                TempData["ErrorMessage"] = "There is no customer profile to show.";
                return RedirectToAction("Index", "Home");
            }
            var customerVM = customer.Data;
            var bookingsResponse = await _bookingService.GetBookingsByCustomerIdAsync(currentUserId);
            List<BookingViewModel> customerBookings = new List<BookingViewModel>();

            var profileVM = new ProfileViewModel
            {
                Customer = customerVM,
                Bookings = customerBookings
            };

            return View(profileVM);
        }


        // GET: Customers/Create
        //ADMINS CAN CREATE USERS
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FirstName,LastName,Email,PhoneNumber")] CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(customerViewModel);
        }

        // GET: Customers/Edit/5
        //AUTHORIZATION: Only admins and the own customer should be able to edit customer details
        [Authorize]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerDetailsAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            var customerViewModel = customer.Data;
            return View(customerViewModel);
        }

        // POST: Customers/Edit/5
        //AUTHOIZATION
        //SHOW CONFIRMATION MESSAGE AFTER EDITING
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(string id, [Bind("CustomerId,FirstName,LastName,Email,PhoneNumber")] CustomerViewModel customerViewModel)
        {
            if (id != customerViewModel.CustomerId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(customerViewModel);
            }
            var success = await _customerService.UpdateCustomerAsync(customerViewModel);
            if (success)
            {
                TempData["SuccessMessage"] = "Profile updated successfully."; // Confirmation message after editing
                return RedirectToAction(nameof(Index));
            } else
            {
                TempData["ErrorMessage"] = "An error occurred while updating the profile.";
            }
                return View(customerViewModel);
        }

        // GET: Customers/Delete/5
        // SHOW CONFIRMATION MESSAGE BEFORE DELETING
        [Authorize]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerDetailsAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer.Data);
        }

        // POST: Customers/Delete/5
        //AUTHORIZATION: Only admins should be able to delete customers
        // SHOW CONFIRMATION MESSAGE AFTER DELETING
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var success = await _customerService.DeleteCustomerAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Customer deleted successfully."; // Confirmation message after deletion
            }
            else
            {
                TempData["ErrorMessage"] = "Customer not found or failed to delete.";
                return NotFound();
            }

            return RedirectToAction("Admin", "Home");
        }
    }
}
