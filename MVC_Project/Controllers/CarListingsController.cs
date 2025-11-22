using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models;
using MVC_Project.Services;

namespace MVC_Project.Controllers
{
    public class CarListingController : Controller
    {
        private readonly ICarClientService _carService;

        public CarListingController(ICarClientService carService)
        {
            _carService = carService;
        }

        // GET: CarListings
        // Fetch List of all Car Listings

        public async Task<IActionResult> Index()
        {
            var cars = await _carService.GetAllCarsAsync();
            if (cars == null || !cars.Success)
            {
                TempData["ErrorMessage"] = "Failed to load car listings. Please try again later.";
                return View(new List<CarListingViewModel>());
            }
            return View(cars.Data);
        }

        // GET: CarListings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var car = await _carService.GetCarDetailsAsync(id.Value);

            if (car == null)
            {
                return NotFound();
            }
            var carViewModel = car.Data;
            return View(carViewModel);
        }

        // GET: CarListings/Create
        //AUTHORIZATION: Only admins should be able to create new car listings
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CarListings/Create
        //AUTHORIZATION : Only admins should be able to create new car listings
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("CarId,Make,Model, PicturesRaw, isAvailable")] CarListingViewModel carListingViewModel)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(carListingViewModel.PicturesRaw))
                {
                    // Split the raw input into a list of picture URLs
                    carListingViewModel.Pictures = carListingViewModel.PicturesRaw
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim())
                        .ToList();
                }

                TempData["SuccessMessage"] = "Listing created successfully."; // Confirmation message after creation
                return RedirectToAction(nameof(Index));
            }
            return View(carListingViewModel);
        }

        // GET: CarListings/Edit/5
        //AUTHORIZATION: Only admins should be able to edit car listings
        [Authorize(Roles = "Admin, SuperUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _carService.GetCarDetailsAsync(id.Value);

            if (car == null)
            {
                return NotFound();
            }
            var carListingViewModel = car.Data;

            // Convert the list of pictures to a raw string for editing, so we dont have to re-add the images on edit
            carListingViewModel.PicturesRaw = string.Join(Environment.NewLine, carListingViewModel.Pictures ?? new List<string>());

            return View(carListingViewModel);
        }

        // POST: CarListings/Edit/5
        //AUTHORIZATION: Only admins should be able to edit car listings
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, SuperUser")]
        public async Task<IActionResult> Edit(int id, [Bind("CarId,Make,Model, PicturesRaw, isAvailable")] CarListingViewModel carListingViewModel)
        {
            if (id != carListingViewModel.CarId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(carListingViewModel);
            }

            if (!string.IsNullOrEmpty(carListingViewModel.PicturesRaw))
            {
                // Split the raw input into a list of picture URLs
                carListingViewModel.Pictures = carListingViewModel.PicturesRaw
                    .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToList();
            }
            var success = await _carService.UpdateCarAsync(carListingViewModel);

            if (success)
            {
                TempData["SuccessMessage"] = "Listing updated successfully."; // Confirmation message after update
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update car listing. Please try again.";
                return NotFound();
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: CarListings/Delete/5
        //AUTHORIZATION: Only admins should be able to delete car listings
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _carService.GetCarDetailsAsync(id.Value);

            if (car == null)
            {
                return NotFound();
            }
            return View(car.Data);
        }

        // POST: CarListings/Delete/5
        //AUTHORIZATION: Only admins should be able to delete car listings
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _carService.DeleteCarAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Listing deleted successfully."; // Confirmation message after deletion
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete car listing. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
