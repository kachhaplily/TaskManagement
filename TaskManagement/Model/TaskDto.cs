using System.ComponentModel.DataAnnotations;

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
        public TaskPriority Priority { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "Due date must be a future date")]
        public DateTime DueDate { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now.Date;

        public bool Status { get; set; }




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
        public  enum TaskPriority
        {
            Low,
            Medium,
            High
        }

    }
}
