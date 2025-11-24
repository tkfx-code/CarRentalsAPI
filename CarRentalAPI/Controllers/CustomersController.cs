using System.Security.Claims;
using AutoMapper;
using CarRentalAPI.Data;
using CarRentalAPI.Dto;
using CarRentalAPI.Interfaces;
using CarRentalAPI.Repositories;
using CarRentalsClassLibrary.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerRepo _repo;
        private readonly IMapper _mapper;

        public CustomersController(ApplicationDbContext context, ICustomerRepo repo, IMapper mapper)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
        }

        // GET: api/<CustomersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomerAsync()
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customers = await _context.Customers.ToListAsync();
            var customerDto = _mapper.Map<IEnumerable<CustomerDto>>(customers);

            return Ok(customerDto);
        }

        //UPDATE SO CURRENT USER CAN UPDATE THEIR OWN INFO
        // PUT api/<CarListingController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, SuperUser")]
        public async Task<IActionResult> UpdateCustomerAsync(string id, CustomerDto customer)
        {

            // Get APIUser ID från JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            // Auktoriseringskontroll: Endast Admin/SuperUser ELLER kunden som äger bokningen får se detaljer.
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperUser") && currentUserId != customer.Id)
            {
                return Forbid();
            }

            if (id != customer.Id)
            {
                return BadRequest();
            }
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repo.CustomerExistsAsync(id))
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

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CustomerDto>> GetCustomerByIdAsync(string id)
        {
            var customer = await _repo.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            var customerDto = _mapper.Map<CustomerDto>(customer);

            return Ok(customerDto);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomerAsync(CustomerDto customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);
            await _repo.AddCustomerAsync(customer);
            var createdCustomerDto = _mapper.Map<CustomerDto>(customer);
            return CreatedAtAction(nameof(GetCustomerByIdAsync), new { id = createdCustomerDto.Id }, createdCustomerDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerAsync(string id)
        {
            var customer = await _repo.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            await _repo.DeleteCustomerAsync(customer.Id);
            return NoContent();
        }

    }

}