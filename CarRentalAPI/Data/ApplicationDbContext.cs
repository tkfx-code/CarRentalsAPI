using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarRentalsClassLibrary.Model;
using Microsoft.AspNetCore.Identity;
using CarRentalAPI.Constants;
using Microsoft.AspNetCore.SignalR;

namespace CarRentalAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<APIUser> //IdentityDbContext<APIUser>?
    {
        public DbSet<CarListing> CarListings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
