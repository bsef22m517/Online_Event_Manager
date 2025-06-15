using EventHub.Models;
using EventHubApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace EventHubApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext <AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EventHubApplication;Trusted_Connection=True;ConnectRetryCount=0");
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<RSVP> RSVPs { get; set; }

    }

}
