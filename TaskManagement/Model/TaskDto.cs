namespace TaskManagement.Model
{
    public class TaskDto
    {

        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
        public bool Status { get; set; }
        public DateTime CreationDate { get; set; }
        public User User { get; set; }
    }
}
