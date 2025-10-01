using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;
using TmsSystem.Models; // ApplicationUser הנכון

namespace TmsSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourBooking> TourBookings { get; set; }
        public DbSet<Itinerary> Itineraries { get; set; }
        public DbSet<ItineraryItem> ItineraryItems { get; set; }
        public DbSet<TourInclude> TourIncludes { get; set; }
        public DbSet<TourExclude> TourExcludes { get; set; }
        public DbSet<PaymentMethod> PaymentsMethod { get; set; }
        public DbSet<Guide> Guides { get; set; } // <-- הוסף שורה זו
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Offer> Offers { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // שמות טבלאות מדויקים במסד
            builder.Entity<TourInclude>().ToTable("tourInclude");
            builder.Entity<TourExclude>().ToTable("tourExclude");
            builder.Entity<ItineraryItem>().ToTable("ItineraryItems"); // אם צריך
         


            // IdentityUserLogin - מפתח קומפוזיט
            builder.Entity<IdentityUserLogin<string>>()
                   .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            // TourBooking - מפתח ראשי
            builder.Entity<TourBooking>()
                   .HasKey(tb => tb.BookingId);

            // TourInclude - מפתח ראשי
            builder.Entity<TourInclude>()
                   .HasKey(ti => ti.Id);

            // TourExclude - מפתח ראשי
            builder.Entity<TourExclude>()
                   .HasKey(te => te.Id);

            //Cascade relationships

            builder.Entity<Tour>()
     .HasMany(t => t.Schedule)
     .WithOne(s => s.Tour)
     .HasForeignKey(s => s.TourId)
     .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Tour>()
                .HasMany(t => t.Includes)
                .WithOne(i => i.Tour)
                .HasForeignKey(i => i.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Tour>()
                .HasMany(t => t.Excludes)
                .WithOne(e => e.Tour)
                .HasForeignKey(e => e.TourId)
                .OnDelete(DeleteBehavior.Cascade);

          




            builder.Entity<Tour>()
    .HasMany(t => t.Excludes)
    .WithOne(e => e.Tour)
    .HasForeignKey(e => e.TourId)
    .OnDelete(DeleteBehavior.Cascade);

           

        }
    }
}
