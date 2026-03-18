using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255)] // <--- CRITICAL: Prevents database truncation
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Specialist";
    }
}