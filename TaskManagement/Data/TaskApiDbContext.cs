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
        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Tasks>()
                .HasOne(t => t.User) // Navigation property in the Task entity
                .WithMany(u => u.Tasks) // Navigation property in the User entity
                .HasForeignKey(t => t.UserId); // Foreign key property in the Task entity // Foreign key property in the Task entity
    }
}
