using Microsoft.EntityFrameworkCore;

namespace TaskTypeApi.Models
{
  public class TaskTypeContext : DbContext
  {
    public TaskTypeContext(DbContextOptions<TaskTypeContext> options) : base(options)
    {
    }
    public DbSet<TaskType> TaskTypes { get; set; }

  }
}