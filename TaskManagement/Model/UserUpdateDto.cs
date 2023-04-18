using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Model
{
    public class UserUpdateDto
    {
        [MinLength(2, ErrorMessage = "Your Name is to short Must be more then 2 characters")]
        public string FirstName { get; set; }
        [MinLength(2, ErrorMessage = "Your Name is to short Must be more then 2 characters")]

        public string LastName { get; set; }
    }
}
