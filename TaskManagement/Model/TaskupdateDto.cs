using static TaskManagement.Model.TaskDto;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Model
{
    public class TaskupdateDto
    {
       
        [MinLength(2, ErrorMessage = "your title must be more then 2 characters")]
        public string Title { get; set; }
       
        [MinLength(2, ErrorMessage = "your title must be more then 2 characters")]
        public string Description { get; set; }

  
        public TaskPriority Priority { get; set; }
    
        [FutureDate(ErrorMessage = "Due date must be a future date")]
        public DateTime DueDate { get; set; }
        public bool Status { get; set; }
    }
}
