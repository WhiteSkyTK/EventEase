using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string TypeName { get; set; } = string.Empty; // e.g., Wedding, Conference

        public ICollection<Event>? Events { get; set; }
    }
}