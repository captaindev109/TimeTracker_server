using Microsoft.EntityFrameworkCore;

namespace CompanyApi.Models
{
  public class CompanyContext : DbContext
  {
    public CompanyContext(DbContextOptions<CompanyContext> options) : base(options)
    {
    }
    public DbSet<Company> Projects { get; set; }

  }
}