namespace Taskify.Models;

public enum TaskStatus { Pending, InProgress, Completed, Cancelled }

public class  TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public TaskStatus Status { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}