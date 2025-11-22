using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;
using TmsSystem.Models;

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
        public DbSet<ApplicationUser> Users { get; set; }
        // שינוי כאן - PaymentMethods במקום PaymentsMethod
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<Guide> Guides { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Offer> Offers { get; set; }
        // Trips - טיולים
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripDay> TripDays { get; set; }
        public DbSet<TripOffer> TripOffers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // שמות טבלאות מדויקים במסד
            builder.Entity<TourInclude>().ToTable("tourInclude");
            builder.Entity<TourExclude>().ToTable("tourExclude");
            builder.Entity<ItineraryItem>().ToTable("ItineraryItems");

            // הגדרת טבלת PaymentMethod
            builder.Entity<PaymentMethod>()
                .ToTable("paymentmethod");

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

            builder.Entity<Trip>()
                .HasOne(t => t.Guide)
                .WithMany(g => g.Trips)
                 .HasForeignKey(t => t.GuideId)
                 .OnDelete(DeleteBehavior.SetNull);

            // Cascade relationships
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



            // Customer
            builder.Entity<Customer>()
                .ToTable("customers");

            // PaymentMethod
            builder.Entity<PaymentMethod>()
                .ToTable("paymentmethod")
                .HasKey(pm => pm.ID);

            // Trip
            builder.Entity<Trip>()
                .ToTable("trips");

            builder.Entity<Trip>()
                .HasMany(t => t.TripDays)
                .WithOne(td => td.Trip)
                .HasForeignKey(td => td.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            // TripDay
            builder.Entity<TripDay>()
                .ToTable("tripdays");

            // TripOffer
            builder.Entity<TripOffer>()
                .ToTable("tripoffers");

            builder.Entity<TripOffer>()
                .HasOne(to => to.Customer)
                .WithMany()
                .HasForeignKey(to => to.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TripOffer>()
                .HasOne(to => to.Trip)
                .WithMany()
                .HasForeignKey(to => to.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TripOffer>()
                .HasOne(to => to.PaymentMethod)
                .WithMany()
                .HasForeignKey(to => to.PaymentMethodId)
                .HasPrincipalKey(pm => pm.ID)
                .OnDelete(DeleteBehavior.Restrict);



            builder.Entity<Trip>()
        .ToTable("trips");

            // TripDay - הגדרת שם טבלה מדויק
            builder.Entity<TripDay>()
                .ToTable("tripdays");

            // Trip relationships
            builder.Entity<Trip>()
                .HasMany(t => t.TripDays)
                .WithOne(td => td.Trip)
                .HasForeignKey(td => td.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TripDay>()
                .HasIndex(td => new { td.TripId, td.DayNumber })
                .IsUnique();


        }
    }
}
