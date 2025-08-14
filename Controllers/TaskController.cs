using System.Security.Claims;
using Microsoft.VisualBasic;

namespace Taskify.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly TaskifyDbContext _db;

    public TaskController(TaskifyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        var tasks = await _db.Tasks.Where(t => t.UserId.ToString() == userId).ToListAsync();
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskItemDto dto, CancellationToken ct)
    {
        if (dto == null || string.IsNullOrEmpty(dto.Title))
        {
            return BadRequest("Task cannot be null or empty.");
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        dto.UserId = int.Parse(userId);
        var taskItem = new TaskItem{
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Status = dto.Status,
            UserId = (int)dto.UserId
        };
        await _db.Tasks.AddAsync(taskItem);
        await _db.SaveChangesAsync();
        return Ok("Task created successfully.");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItemDto task)
    {
        if (task == null || string.IsNullOrEmpty(task.Title))
        {
            return BadRequest("Task cannot be null or empty.");
        }
        var existingTask = await _db.Tasks.FindAsync(id);
        if (
            existingTask == null
            || existingTask.UserId.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier)
        )
        {
            return NotFound();
        }
        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.DueDate = task.DueDate;
        existingTask.Status = task.Status;
        await _db.SaveChangesAsync();
        return Ok("Task updated successfully!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (
            task == null
            || task.UserId.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier)
        )
        {
            return NotFound();
        }
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
