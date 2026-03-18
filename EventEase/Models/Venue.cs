using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EventEase.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "❌ Please provide a name for this venue.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Venue Name")]
        public string VenueName { get; set; } = string.Empty;

        [Required(ErrorMessage = "❌ A location is required.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "❌ Capacity is required.")]
        [Range(1, 100000, ErrorMessage = "❌ Capacity must be greater than 0!")]
        public int Capacity { get; set; }

        public string ImageUrl { get; set; } = "https://via.placeholder.com/150";

        // Navigation properties
        public ICollection<Event>? Events { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}