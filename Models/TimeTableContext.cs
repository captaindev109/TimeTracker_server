using Microsoft.EntityFrameworkCore;

namespace TimeTableApi.Models
{
  public class TimeTableContext : DbContext
  {
    public TimeTableContext(DbContextOptions<TimeTableContext> options) : base(options)
    {
    }
    public DbSet<TimeTable> TimeTables { get; set; }

  }
}