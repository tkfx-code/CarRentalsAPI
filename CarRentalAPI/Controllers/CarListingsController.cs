using CarRentalAPI.Data;
using CarRentalAPI.Interfaces;
using CarRentalsClassLibrary.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//not started

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarListingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICarListingRepo _repo;

        public CarListingsController(ApplicationDbContext context, ICarListingRepo repo)
        {
            _context = context;
            _repo = repo;
        }


        // GET: api/<CarListingController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarListing>>> GetAllCarsAsync()
        {
            if (_context.CarListings == null)
            {
                return NotFound();
            }
            return await _context.CarListings.ToListAsync();
        }

        // PUT api/<CarListingController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, SuperUser")]
        public async Task<IActionResult> UpdateCarAsync(int id, CarListing carListing)
        {
            if (id != carListing.CarId)
            {
                return BadRequest();
            }    
            _context.Entry(carListing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repo.CarExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // GET api/<CarListingController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CarListingController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        

        // DELETE api/<CarListingController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
