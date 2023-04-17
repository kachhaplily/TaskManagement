using Microsoft.EntityFrameworkCore;
using TaskManagement.Model;

namespace TaskManagement.Data
{
    public class TaskApiDbContext:DbContext
    {
        public TaskApiDbContext(DbContextOptions option) : base(option)
        {
            
        }
        public DbSet<User> Users{ get; set; }
        public DbSet<Tasks> Tasks { get; set; }
    }
}
