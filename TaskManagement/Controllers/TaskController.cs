using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagement.Data;
using TaskManagement.Model;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : Controller
    {
        private readonly TaskApiDbContext dbContext;

        public TaskController(TaskApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }




        // getTheAllTask 

        [HttpGet("AllTask"), Authorize]


        public async Task<ActionResult> GetAllTask()
        {
            return Ok(await dbContext.Tasks.ToListAsync());
        }



        //task get by UserId
        [HttpGet("{userId}/tasks")]
        public async Task<ActionResult<List<Tasks>>> GetTasksByUserId(int userId)
        {
            // Find all tasks associated with the specified user ID
            var tasks = await dbContext.Tasks.Where(t => t.UserId == userId).ToListAsync();

            // Return the list of tasks with a 200 OK status code
            return Ok(tasks);
        }


        //Task delete

        [HttpDelete("{userId}/tasks/{taskId}")]
        public async Task<ActionResult> DeleteTask(int userId, int taskId)
        {
            var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.UserId == userId && x.TaskId == taskId);

            if (task != null)
            {
                dbContext.Tasks.Remove(task);
                await dbContext.SaveChangesAsync();
                return Ok("Task deleted successfully");
            }
            return NotFound();
        }





        //    userRegistration 
        [HttpPost("Task"), Authorize]
        public async Task<ActionResult> CreateTask(TaskDto taskDto, int userId)
        {
            // Map the TaskDto object to a Task object
            var task = new Tasks
            {
                UserId = userId,
                Title = taskDto.Title,
                Description = taskDto.Description,
                Priority = taskDto.Priority,
                DueDate = taskDto.DueDate,
                Status = taskDto.Status,
                CreationDate = taskDto.CreationDate,
            };

            // Add the new task to the database
            await dbContext.Tasks.AddAsync(task);
            await dbContext.SaveChangesAsync();

            // Return the created task with a 201 Created status code
            return Ok(task);
        }


        //TaskUpdate
        [HttpPut("updateTask")]
        public async Task<ActionResult>UpdateTask( int userId, int taskId, TaskupdateDto taskupdate)
        {
            var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.UserId == userId && x.TaskId == taskId);

            if (task != null)
            {
                task.Title = taskupdate.Title;
                task.Description = taskupdate.Description;
                task.Priority = taskupdate.Priority;
                task.DueDate = taskupdate.DueDate;
                task.Status = taskupdate.Status;
                await dbContext.SaveChangesAsync();
                return Ok(task);
            }
            return NotFound();
        }




    }
}
