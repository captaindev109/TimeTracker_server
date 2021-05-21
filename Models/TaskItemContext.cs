using Microsoft.EntityFrameworkCore;

namespace TaskItemApi.Models
{
  public class TaskItemContext : DbContext
  {
    public TaskItemContext(DbContextOptions<TaskItemContext> options) : base(options)
    {
    }
    public DbSet<TaskItem> TaskItems { get; set; }

  }
}