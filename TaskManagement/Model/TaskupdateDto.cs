﻿using static TaskManagement.Model.TaskDto;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Model
{
    public class TaskupdateDto
    {
        [Required]
        [MinLength(2, ErrorMessage = "your title must be more then 2 characters")]
        public string Title { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "your title must be more then 2 characters")]
        public string Description { get; set; }

        [Range(1, 10, ErrorMessage = "Priority must be between 1 and 10")]
        public int Priority { get; set; }
        [Required]
        [FutureDate(ErrorMessage = "Due date must be a future date")]
        public DateTime DueDate { get; set; }
        public bool Status { get; set; }
    }
}