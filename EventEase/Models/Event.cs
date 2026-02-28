using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string EventName { get; set; }

        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        // Nullable because events can be loaded before a venue is assigned
        public int? VenueId { get; set; }

        [ForeignKey("VenueId")]
        public Venue Venue { get; set; }

        // Navigation property
        public ICollection<Booking> Bookings { get; set; }
    }
}
