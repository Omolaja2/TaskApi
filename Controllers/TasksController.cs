using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Data;
using TaskFlowAPI.Models;

namespace TaskFlowAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;
    private static readonly string[] AllowedStatuses = new[] { "pending", "in progress", "completed" };

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskItem task)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Task data is invalid.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

        if (!AllowedStatuses.Contains(task.Status.Trim(), StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { message = "Invalid task status. Allowed values are: pending, in progress, completed." });

        task.Status = task.Status.Trim().ToLowerInvariant();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Task created successfully",
            data = task
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks(
        int page = 1,
        int pageSize = 5,
        string? status = null)
    {
        var query = _context.Tasks.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(t => t.Status == status);
        }

        var totalTasks = await query.CountAsync();

        var tasks = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            totalTasks,
            page,
            pageSize,
            data = tasks
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound(new { message = "Task not found" });

        return Ok(task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, TaskItem updatedTask)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Task data is invalid.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

        if (!AllowedStatuses.Contains(updatedTask.Status.Trim(), StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { message = "Invalid task status. Allowed values are: pending, in progress, completed." });

        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound(new { message = "Task not found" });

        task.Title = updatedTask.Title;
        task.Description = updatedTask.Description;
        task.Status = updatedTask.Status.Trim().ToLowerInvariant();
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Task updated successfully",
            data = task
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound(new { message = "Task not found" });

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Task deleted successfully"
        });
    }
}