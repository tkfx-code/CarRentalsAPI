using AutoMapper;
using CarRentalAPI.Data;
using CarRentalAPI.Dto;
using CarRentalAPI.Interfaces;
using CarRentalsClassLibrary.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarListingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICarListingRepo _repo;
        private readonly IMapper _mapper;

        public CarListingsController(ApplicationDbContext context, ICarListingRepo repo, IMapper mapper)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
        }


        // GET: api/<CarListingController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarListingDto>>> GetAllCarsAsync()
        {
            if (_context.CarListings == null)
            {
                return NotFound();
            }
            var carListings = await _context.CarListings.ToListAsync();
            var carListingDtos = _mapper.Map<IEnumerable<CarListingDto>>(carListings);

            return Ok(carListingDtos);
        }

        // PUT api/<CarListingController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, SuperUser")]
        public async Task<IActionResult> UpdateCarAsync(int id, CarListingDto carListingDto)
        {
            if (id != carListingDto.CarId)
            {
                return BadRequest();
            }    
            var carListing = _mapper.Map<CarRentalsClassLibrary.Model.CarListing>(carListingDto);

            _context.Entry(carListingDto).State = EntityState.Modified;

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
        public async Task<ActionResult<CarListingDto>> GetCar(int id)
        {
            var car = await _repo.GetCarByIdAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            var caDto = _mapper.Map<CarListingDto>(car);

            return Ok(caDto);
        }

        // POST api/<CarListingController>
        [HttpPost]
        public async Task<ActionResult<CarListingDto>> Post(CarListingDto carListingDto)
        {
            var car = _mapper.Map<CarListing>(carListingDto);
            await _repo.AddCarAsync(car);
            var createdCarDto = _mapper.Map<CarListingDto>(car);
            return CreatedAtAction(nameof(GetCar), new { id = createdCarDto.CarId }, createdCarDto);

        }

        

        // DELETE api/<CarListingController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _repo.GetCarByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            await _repo.DeleteCarAsync(customer.CarId);
            return NoContent();
        }
    }
}
