using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static TaskManagement.Model.TaskDto;

namespace TaskManagement.Model
{
    public class Tasks
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Enumdata.TaskPriority Priority { get; set; } = Enumdata.TaskPriority.Medium;
        public DateTime DueDate { get; set; }
        public Enumdata.TaskStatus Status { get; set; } = Enumdata.TaskStatus.Todo;
        public DateTime CreationDate { get; set; }
        public virtual User User { get; set; } // Navigation property


    }


}
