using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace EventEase.Models
{
    // 1. Notice the ": IValidatableObject" added here
    public class Booking : IValidatableObject
    {
        [Key]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Please enter the customer's name")]
        [Display(Name = "First Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the customer's surname")]
        [Display(Name = "Surname")]
        public string CustomerSurname { get; set; } = string.Empty;

        [Required(ErrorMessage = "A contact number is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")] // Added a custom message here!
        [Display(Name = "Phone Number")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        // Fixes the "Year 0" issue by defaulting to today's date
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [Required]
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        [Required]
        public int VenueId { get; set; }
        [ForeignKey("VenueId")]
        public Venue? Venue { get; set; }

        // 2. The Validate method is now safely INSIDE the Booking class
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BookingDate.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "🛑 Reservations cannot be made for past dates!",
                    new[] { nameof(BookingDate) });
            }
        }
    }
}