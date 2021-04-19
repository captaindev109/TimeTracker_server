using Microsoft.EntityFrameworkCore;

namespace ResourceTypeApi.Models
{
  public class ResourceTypeContext : DbContext
  {
    public ResourceTypeContext(DbContextOptions<ResourceTypeContext> options) : base(options)
    {
    }
    public DbSet<ResourceType> Projects { get; set; }

  }
}