using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagement.Model
{

   
    public class TaskDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "your title must be more then 2 characters")]
        public string Title { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "your title must be more then 2 characters")]
        public string Description { get; set; }

        [Required]
        public Enumdata.TaskPriority Priority { get; set; } = Enumdata.TaskPriority.Medium;

        [Required]
        [FutureDate(ErrorMessage = "Due date must be a future date")]
        public DateTime DueDate { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now.Date;

        public Enumdata.TaskStatus Status { get; set; }=Enumdata.TaskStatus.Todo;




       // checked valid due date class

        public class FutureDateAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                DateTime date = Convert.ToDateTime(value);
                return date > DateTime.Now;
            }
        }

        //task  priority enum


       


        

    }
}
