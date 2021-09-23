using TimeTracker_server.Models;
using Microsoft.EntityFrameworkCore;

namespace TimeTracker_server.Data
{
  public class MyDbContext : DbContext
  {
    public MyDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ResourceType> ResourceTypes { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<TaskType> TaskTypes { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<TimeTable> TimeTables { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserAcl> UserAcls { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagAcl> TagAcls { get; set; }
  }
}