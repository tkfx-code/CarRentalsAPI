using System.Diagnostics;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Models;
using MVC_Project.Services;


namespace MVC_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarClientService _carService;          
        private readonly ICustomerClientService _customerService;  
        private readonly IBookingClientService _bookingService;

        public HomeController(ILogger<HomeController> logger, ICarClientService carService, ICustomerClientService customerService, IBookingClientService bookingService)
        {
            _logger = logger;
            _carService = carService;
            _customerService = customerService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            var carListings = await _carService.GetAllCarsAsync();
            //Error testing 
            //
            if (!carListings.Success)
            {
                // Logga felet för backend-debugging
                _logger.LogError("Could not get cars - HomeController: {ErrorMessage}", carListings.Message);

                // Skicka meddelandet till vyn för att informera användaren
                ViewData["ErrorMessage"] = "API error in HomeController.";
            }

            return View(carListings.Data ?? new List<CarListingViewModel>());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var customerViewModel = await _customerService.GetAllCustomersAsync();
            List<CustomerViewModel> customers; 

            if (customerViewModel != null)
            {
                customers = customerViewModel.Data;
            } else
            {
                customers = new List<CustomerViewModel>();
            }

            var bookingViewModel = await _bookingService.GetAllBookingsAsync();
            List<BookingViewModel> bookings;

            if (bookingViewModel != null)
            {
                bookings = bookingViewModel.Data;
            }
            else
            {
                bookings = new List<BookingViewModel>();
            }

            var adminViewModel = new AdminViewModel
                {
                    Customers = customers,
                    Bookings = bookings
                };
            return View(adminViewModel);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
