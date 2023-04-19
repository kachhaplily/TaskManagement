using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Model
{
    public class userloginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        public string Password { get; set; }

    }
}
