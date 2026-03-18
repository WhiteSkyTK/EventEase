using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "❌ An email address is required.")]
        [EmailAddress(ErrorMessage = "❌ Please enter a valid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "❌ A temporary password is required.")]
        [MinLength(6, ErrorMessage = "❌ Password must be at least 6 characters long.")]
        [MaxLength(255)] // Prevents database truncation issues with the hash!
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Specialist";
    }
}