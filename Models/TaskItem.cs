using System.ComponentModel.DataAnnotations;

namespace TaskFlowAPI.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = default!;


    [Required]
    [StringLength(500)]
    public string Description { get; set; } = default!;


    [Required]
    public string Status { get; set; } = "pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}