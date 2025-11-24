using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using CarRentalAPI.Interfaces;
using CarRentalAPI.Dto;
using CarRentalsClassLibrary.Model;
using System.Security.Claims;
using CarRentalAPI.Data;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ICarListingRepo _carRepo;
        private readonly IBookingRepo _bookingRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly UserManager<APIUser> _userManager;
        private readonly IMapper _mapper;

        public BookingsController(ICarListingRepo carListingRepo, IBookingRepo bookingRepo, ICustomerRepo customerRepo, UserManager<APIUser> userManager, IMapper mapper)
        {
            _carRepo = carListingRepo;
            _bookingRepo = bookingRepo;
            _customerRepo = customerRepo;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: api/Bookings Get all bookings
        [HttpGet]
        [Authorize(Roles = "Admin, SuperUser")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            // This method has to be implemented with .Include() in repo
            var bookings = await _bookingRepo.GetAllBookingsAsync();

            var bookingDtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);

            return Ok(bookingDtos);
        }

        // GET: api/Bookings/5 - Fetch specific booking
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            var booking = await _bookingRepo.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            // Get APIUser ID från JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            // Auktoriseringskontroll: Endast Admin/SuperUser ELLER kunden som äger bokningen får se detaljer.
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperUser") && currentUserId != booking.CustomerId)
            {
                return Forbid();
            }

            var bookingDto = _mapper.Map<BookingDto>(booking);

            return Ok(bookingDto);
        }

        // POST: api/Bookings - Create new booking
        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<BookingDto>> PostBooking(BookingDto createBookingDto)
        {
            // Validate car 
            var car = await _carRepo.GetCarByIdAsync(createBookingDto.CarId);
            if (car == null)
            {
                ModelState.AddModelError("CarId", "The specified car was not found.");
                return BadRequest(ModelState);
            }

            // 2. Fetch Id from log in 
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("Customer profile not linked or found.");
            }

            if (ModelState.IsValid)
            {
                var booking = _mapper.Map<Booking>(createBookingDto);
                booking.CustomerId = currentUserId;

                await _bookingRepo.AddBookingAsync(booking);

                var createdBooking = await _bookingRepo.GetBookingByIdAsync(booking.BookingId);

                return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, _mapper.Map<BookingDto>(createdBooking));
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _bookingRepo.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();

            // Hämta Identity User ID (string) direkt från JWT-token
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Auktoriseringskontroll: Jämför direkt mot bokningens FK
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperUser") && currentUserId != booking.CustomerId)
            {
                return Forbid();
            }

            // 2. Utför radering
            await _bookingRepo.DeleteBookingAsync(id);
            return NoContent();
        }

        [HttpPut ("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBooking(int id, BookingDto bookingDto)
        {
            if (id != bookingDto.BookingId)
            {
                return BadRequest("Booking ID mismatch.");
            }

            var existingBooking = await _bookingRepo.GetBookingByIdAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperUser") && currentUserId != existingBooking.CustomerId)
            {
                return Forbid();
            }

            var car = await _carRepo.GetCarByIdAsync(bookingDto.CarId);
            if (car == null)
            {
                ModelState.AddModelError("CarId", "The specified car was not found.");
                return BadRequest(ModelState);
            }
            _mapper.Map(bookingDto, existingBooking);
            
            await _bookingRepo.UpdateBookingAsync(existingBooking);
            return NoContent();
        }

        // GET: api/Bookings/ByCustomer/{customerId}
        [HttpGet("ByCustomer/{customerId}")] 
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByCustomer(string customerId)
        {
           
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperUser") && currentUserId != customerId)
            {
                return Forbid();
            }

            var bookings = await _bookingRepo.GetBookingsByCustomerIdAsync(customerId);

            var bookingDtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);

            return Ok(bookingDtos);
        }

    }   
}
