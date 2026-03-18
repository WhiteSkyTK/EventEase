using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace EventEase.Models
{
    // Added IValidatableObject for custom date logic
    public class Event : IValidatableObject
    {
        [Key]
        public int EventId { get; set; }

        [Required(ErrorMessage = "❌ Event Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "❌ Please provide a description.")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Event Poster")]
        public string? ImageUrl { get; set; }

        [Required]
        [Display(Name = "Start Date & Time")]
        // FIX: Defaults to today to prevent the "Year 0" bug
        public DateTime StartDateTime { get; set; } = DateTime.Today.AddHours(12);

        [Required]
        [Display(Name = "End Date & Time")]
        // FIX: Defaults to today + 4 hours
        public DateTime EndDateTime { get; set; } = DateTime.Today.AddHours(16);

        public int? VenueId { get; set; }

        [ForeignKey("VenueId")]
        public virtual Venue? Venue { get; set; }

        public virtual ICollection<Booking>? Bookings { get; set; }

        // --- CUSTOM VALIDATION ---
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDateTime <= StartDateTime)
            {
                yield return new ValidationResult(
                    "🛑 The End Date/Time must be AFTER the Start Date/Time!",
                    new[] { nameof(EndDateTime) });
            }
        }
    }
}