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
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Guide> Guides { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripDay> TripDays { get; set; }
        public DbSet<TripOffer> TripOffers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ===== שמות טבלאות =====
            builder.Entity<TourInclude>().ToTable("tourinclude");
            builder.Entity<TourExclude>().ToTable("tourexclude");
            builder.Entity<ItineraryItem>().ToTable("itineraryitems");
            builder.Entity<Customer>().ToTable("customers");
            builder.Entity<PaymentMethod>().ToTable("paymentmethod");
            builder.Entity<Guide>().ToTable("guides");
            builder.Entity<Trip>().ToTable("trips");
            builder.Entity<TripDay>().ToTable("tripdays");
            builder.Entity<TripOffer>().ToTable("tripoffers");

            // ===== Identity - IdentityUserLogin =====
            builder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            // ===== TourBooking =====
            builder.Entity<TourBooking>()
                .HasKey(tb => tb.BookingId);

            // ===== TourInclude =====
            builder.Entity<TourInclude>()
                .HasKey(ti => ti.Id);

            // ===== TourExclude =====
            builder.Entity<TourExclude>()
                .HasKey(te => te.Id);

            // ===== PaymentMethod =====
            builder.Entity<PaymentMethod>()
                .HasKey(pm => pm.ID);

            // ===== Guide =====
            builder.Entity<Guide>()
                .HasKey(e => e.GuideId);

            // ===== Tour - קשרים לטיולים רגילים =====
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

            // ===== Trip - טיולים מאורגנים =====
            builder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.TripId);

                // קשר למדריך
                entity.HasOne(t => t.Guide)
                    .WithMany(g => g.Trips)
                    .HasForeignKey(t => t.GuideId)
                    .OnDelete(DeleteBehavior.SetNull);

                // קשר ל-TripDays
                entity.HasMany(t => t.TripDays)
                    .WithOne(td => td.Trip)
                    .HasForeignKey(td => td.TripId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ✅ קשר ל-TripOffers
                entity.HasMany(t => t.TripOffers)
                    .WithOne(to => to.Trip)
                    .HasForeignKey(to => to.TripId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== TripDay =====
            builder.Entity<TripDay>(entity =>
            {
                entity.HasKey(e => e.TripDayId);

                // אינדקס ייחודי - טיול + מספר יום
                entity.HasIndex(td => new { td.TripId, td.DayNumber })
                    .IsUnique();
            });

            // ===== TripOffer - הצעות מחיר לטיולים =====
            builder.Entity<TripOffer>(entity =>
            {
                entity.HasKey(e => e.TripOfferId);

                // קשר ללקוח
                entity.HasOne(to => to.Customer)
                    .WithMany()
                    .HasForeignKey(to => to.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ✅ הקשר הזה מוגדר כבר מצד Trip, אז לא צריך כאן
                // אבל אפשר להשאיר לבהירות
                entity.HasOne(to => to.Trip)
                    .WithMany(t => t.TripOffers)
                    .HasForeignKey(to => to.TripId)
                    .OnDelete(DeleteBehavior.Restrict);

                // קשר לאמצעי תשלום
                entity.HasOne(to => to.PaymentMethod)
                    .WithMany()
                    .HasForeignKey(to => to.PaymentMethodId)
                    .HasPrincipalKey(pm => pm.ID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}