using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Model
{
    public class User
    {

   
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

     
        public string Email { get; set; } 
   
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string? FirstName { get; set; }
   
        public string? LastName { get; set; }
            
        public string? Token { get; set; }

      
        public string? ResetPasswordToken { get; set; } // Add this property

        public DateTime ResetPasswordExpiry { get; set; } // Add this property

    }
}
