using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using System;

namespace EventEase.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Staff> Staff { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Seed Venues
            modelBuilder.Entity<Venue>().HasData(
                new Venue { VenueId = 1, VenueName = "The Grand Ballroom", Location = "123 Marble Ave, Cape Town", Capacity = 500, ImageUrl = "https://images.unsplash.com/photo-1519167758481-83f550bb49b3?w=800" },
                new Venue { VenueId = 2, VenueName = "Sunset Garden", Location = "45 Ocean View, Durban", Capacity = 150, ImageUrl = "https://images.unsplash.com/photo-1523438885200-e635ba2c371e?w=800" },
                new Venue { VenueId = 3, VenueName = "Tech Hub Plaza", Location = "88 Innovation Dr, Sandton", Capacity = 1000, ImageUrl = "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800" },
                new Venue { VenueId = 4, VenueName = "Cozy Corner Café", Location = "12 Main St, Pretoria", Capacity = 30, ImageUrl = "https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=800" }
            );

            // 2. Seed Events
            modelBuilder.Entity<Event>().HasData(
                new Event { EventId = 1, EventName = "Annual Charity Gala", Description = "Formal dinner and auction.", StartDateTime = new DateTime(2026, 05, 10, 18, 0, 0), EndDateTime = new DateTime(2026, 05, 10, 23, 59, 0), VenueId = 1 },
                new Event { EventId = 2, EventName = "Cloud Dev Summit", Description = "Technical conference.", StartDateTime = new DateTime(2026, 06, 15, 8, 0, 0), EndDateTime = new DateTime(2026, 06, 15, 17, 0, 0), VenueId = 3 },
                new Event { EventId = 3, EventName = "Smith Wedding", Description = "Private ceremony.", StartDateTime = new DateTime(2026, 07, 20, 14, 0, 0), EndDateTime = new DateTime(2026, 07, 20, 22, 0, 0), VenueId = 2 }
            );

            // 3. Seed Bookings
            modelBuilder.Entity<Booking>().HasData(
                new Booking { BookingId = 1, EventId = 1, VenueId = 1, BookingDate = new DateTime(2026, 05, 10) }
            );

            // 4. Seed Staff (Admin and Specialist)
            modelBuilder.Entity<Staff>().HasData(
                new Staff
                {
                    StaffId = 1,
                    Email = "admin@eventease.com",
                    Password = "Admin123!",
                    Role = "Admin"
                },
                new Staff
                {
                    StaffId = 2,
                    Email = "specialist@eventease.com",
                    Password = "Book123!",
                    Role = "Specialist"
                }
            );
        }
    }
}