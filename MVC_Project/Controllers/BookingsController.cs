using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Text;
using MVC_Project.Services;
using System.Security.Claims;


namespace MVC_Project.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingClientService _bookingService;
        private readonly ICarClientService _carService;
        private readonly ICustomerClientService _customerService;

        public BookingsController(IBookingClientService bookingService, ICarClientService carService, ICustomerClientService customerService)
        {
            _bookingService = bookingService;
            _carService = carService;
            _customerService = customerService;
        }

        // GET: Bookings
        //Fetch List of all Bookings
        [Authorize(Roles = "Admin, SuperUser")]
        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();

            if (bookings == null)
            {
                TempData["ErrorMessage"] = "Unable to retrieve bookings at this time.";
                return View(new List<BookingViewModel>());
            }

            return View(bookings.Data ?? new List<BookingViewModel>());
        }

        // GET: Bookings/Details/5
        // Fetch Details of a specific Booking by BookingID
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _bookingService.GetBookingDetailsAsync(id);

            
            if (booking == null || !booking.Success || booking.Data == null)
            {
                return NotFound();
            }

            var bookingViewModel = booking.Data;

            var carDetails = await _carService.GetCarDetailsAsync(bookingViewModel.CarId);
            if (carDetails != null && carDetails.Success)
            {
                bookingViewModel.Car = carDetails.Data;
            }

            return View(bookingViewModel);
        }

        // GET: Bookings/Create?carId=X
        // FIX: Metoden tar nu emot carId från URL och konstruerar ViewModel
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create(int? carId)
        {
            if (carId == null)
            {
                TempData["ErrorMessage"] = "No car found.";
                return RedirectToAction("Index", "Home");
            }
            //fetch car details
            var carResult = await _carService.GetCarDetailsAsync(carId.Value);
            if (carResult == null || !carResult.Success || carResult.Data == null)
            {
                TempData["ErrorMessage"] = "Car not found or not available.";
                return RedirectToAction("Index", "Home");
            }

            //fetch customer details
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            CustomerViewModel currentCustomer = new CustomerViewModel();

            string customerIdentityId = string.Empty;

            if (!string.IsNullOrEmpty(userIdClaim))
            {
                customerIdentityId = userIdClaim;

                var customerResponse = await _customerService.GetCustomerDetailsAsync(userIdClaim);

                if (customerResponse != null && customerResponse.Success && customerResponse.Data != null)
                {
                    currentCustomer = customerResponse.Data;
                }
            }

            // 3. Skapa ViewModel för vyn
            var bookingViewModel = new BookingViewModel
            {
                CarId = carId.Value,
                Car = carResult.Data,
                CustomerId = customerIdentityId,
                Customer = currentCustomer
            };

            return View(bookingViewModel);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("CarId,StartDate,EndDate,CustomerId")] BookingViewModel bookingViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(bookingViewModel);
            }

            var result = await _bookingService.CreateBookingAsync(bookingViewModel);
            if (result)
            {
                TempData["SuccessMessage"] = "Booking created successfully."; 
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create booking. Please try again.";
                return View(bookingViewModel);
            }
        }

        // GET: Bookings/Edit/5
        // Fetch the Booking to be edited by BookingID
        [Authorize(Roles = "Admin, SuperUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _bookingService.GetBookingDetailsAsync(id.Value);

            if (booking == null)
            {
                return NotFound();
            }

            var bookingViewModel = booking.Data;
            return View(bookingViewModel);
        }

        // POST: Bookings/Edit/5
        // When editing, ensure the BookingId matches the one in the model and return to Index if successful
        //ADD CONFIRMATION MESSAGE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, SuperUser")]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,StartDate,EndDate")] BookingViewModel bookingViewModel)
        {
            if (id != bookingViewModel.BookingId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(bookingViewModel);
            }

            var success = await _bookingService.UpdateBookingAsync(bookingViewModel);

            if (success)
            {
                TempData["SuccessMessage"] = "Booking updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update booking. Please try again.";
                return View(bookingViewModel);
            }
        }

        // GET: Bookings/Delete/5
        // Fetch the Booking to be deleted by BookingID
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {

            var booking = await _bookingService.GetBookingDetailsAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        // Deletes the Booking by BookingID
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _bookingService.DeleteBookingAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Booking not found or failed to delete.";
                return NotFound();
            }

            TempData["SuccessMessage"] = "Booking deleted successfully."; // Confirmation message after deletion
            if (User.IsInRole("Admin") || User.IsInRole("SuperUser"))
            {
                return RedirectToAction("Home", "Admin"); //If admin removes booking return to Admin dashboard
            }
            else
            {
                return RedirectToAction("Profile", "Customers"); //If customer removes their booking return to profile
            }
        }
    }
}
