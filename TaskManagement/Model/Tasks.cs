using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
            public int Priority { get; set; }
            public DateTime DueDate { get; set; }
            public bool Status { get; set; }
            public DateTime CreationDate { get; set; }
          

       
    }

    
}
