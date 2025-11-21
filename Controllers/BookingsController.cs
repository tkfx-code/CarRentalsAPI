//using Microsoft.AspNetCore.Mvc;
//using MVC_Project.Models;
//using Microsoft.AspNetCore.Authorization;
//using AutoMapper;
//using System.Text;
//using MVC_Project.Services;
//using System.Security.Claims;


//namespace MVC_Project.Controllers
//{
//    public class BookingsController : Controller
//    {
//        private readonly IBookingClientService _bookingService;
//        private readonly ICarClientService _carService;

//        public BookingsController(IBookingClientService bookingService, ICarClientService carService)
//        {
//            _bookingService = bookingService;
//            _carService = carService;
//        }

//        //Get token current session
//        private string GetJwtToken()
//        {
//            return HttpContext.Session.GetString("JWToken") ?? string.Empty;
//        }

//        // GET: Bookings
//        //Fetch List of all Bookings
//        [Authorize(Roles = "Admin, SuperUser")]
//        public async Task<IActionResult> Index()
//        {
//            var token = GetJwtToken();
//            var bookings = await _bookingService.GetAllBookingsAsync(token);

//            if (bookings == null)
//            {
//                TempData["ErrorMessage"] = "Unable to retrieve bookings at this time.";
//                return View(new List<BookingViewModel>());
//            }

//            return View(bookings);
//        }

//        // GET: Bookings/Details/5
//        // Fetch Details of a specific Booking by BookingID
//        [Authorize]
//        public async Task<IActionResult> Details(int? id)
//        {
//            var token = GetJwtToken();
//            var booking = await _bookingService.GetBookingDetailsAsync(id, token);

//            if (booking == null)
//            {
//                return NotFound();
//            }
//            var carDetails = await _carService.GetCarDetailsAsync(booking.CarId, token);
//            if (carDetails != null)
//            {
//                booking.Car = carDetails;
//            }
//            return View(booking);
//        }

//        // POST: Bookings/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize]
//        public async Task<IActionResult> Create([Bind("CarId,StartDate,EndDate,CustomerId")] BookingViewModel bookingViewModel)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(bookingViewModel);
//            }
//            var token = GetJwtToken();
//            var result = await _bookingService.CreateBookingAsync(bookingViewModel, token);
//            if (result)
//            {
//                TempData["SuccessMessage"] = "Booking created successfully."; // Confirmation message after booking
//                return RedirectToAction("Index", "Home");
//            }
//            else
//            {
//                TempData["ErrorMessage"] = "Failed to create booking. Please try again.";
//                return View(bookingViewModel);
//            }
//        }

//        // GET: Bookings/Edit/5
//        // Fetch the Booking to be edited by BookingID
//        [Authorize(Roles = "Admin, SuperUser")]
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var booking = await _context.Bookings
//                .Include(b => b.Car)
//                .Include(b => b.Customer)
//                .FirstOrDefaultAsync(b => b.BookingId == id);

//            if (booking == null)
//            {
//                return NotFound();
//            }
//            var bookingViewModel = _mapper.Map<BookingViewModel>(booking);
//            return View(bookingViewModel);
//        }

//        // POST: Bookings/Edit/5
//        // When editing, ensure the BookingId matches the one in the model and return to Index if successful
//        //ADD CONFIRMATION MESSAGE
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize(Roles = "Admin, SuperUser")]
//        public async Task<IActionResult> Edit(int id, [Bind("BookingId,StartDate,EndDate")] BookingViewModel bookingViewModel)
//        {
//            if (id != bookingViewModel.BookingId)
//            {
//                return NotFound();
//            }

//            if (!ModelState.IsValid)
//            {
//                return View(bookingViewModel);
//            }

//            var booking = await _context.Bookings
//                .Include(b => b.Car)
//                .FirstOrDefaultAsync(b => b.BookingId == id);

//            if (booking == null)
//            {
//                return NotFound();
//            }

//            _mapper.Map(bookingViewModel, booking);
//            booking.Car = await _context.CarListings.FindAsync(bookingViewModel.CarId);
//            try
//            {
//                _context.Update(booking);
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!BookingExists(bookingViewModel.BookingId))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }
//            TempData["SuccessMessage"] = "Booking updated successfully."; // Confirmation message after editing
//            return RedirectToAction(nameof(Index));
//        }

//        // GET: Bookings/Delete/5
//        // Fetch the Booking to be deleted by BookingID
//        [Authorize]
//        public async Task<IActionResult> Delete(int? id)
//        {
//            var token = GetJwtToken();
//            var booking = await _bookingService.GetBookingDetailsAsync(id, token);
//            if (booking == null)
//            {
//                return NotFound();
//            }
//            return View(booking);
//        }

//        // POST: Bookings/Delete/5
//        // Deletes the Booking by BookingID
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        [Authorize]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var booking = await _context.Bookings.FindAsync(id);
//            if (booking == null)
//            {
//                return NotFound();
//            }

//            _context.Bookings.Remove(booking);
//            await _context.SaveChangesAsync();

//            TempData["SuccessMessage"] = "Booking deleted successfully."; // Confirmation message after deletion
//            if (User.IsInRole("Admin") || User.IsInRole("SuperUser"))
//            {
//                return RedirectToAction("Home", "Admin"); //If admin removes booking return to Admin dashboard
//            }
//            else
//            {
//                return RedirectToAction("Profile", "Customers"); //If customer removes their booking return to profile
//            }
//        }

//        private bool BookingExists(int id)
//        {
//            return _context.Bookings.Any(e => e.BookingId == id);
//        }
//    }
//}
