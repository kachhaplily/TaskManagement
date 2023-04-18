using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Model
{
    public class UserDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$", ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; }

        [MinLength(2, ErrorMessage = "Your Name is to short Must be more then 2 characters")]
        public string? FirstName { get; set; }
        [MinLength(2, ErrorMessage = "Your Name is to short Must be more then 2 characters")]

        public string? LastName { get; set; }
    }

}
