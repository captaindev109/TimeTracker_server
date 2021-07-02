using Microsoft.EntityFrameworkCore;

namespace UserAclApi.Models
{
  public class UserAclContext : DbContext
  {
    public UserAclContext(DbContextOptions<UserAclContext> options) : base(options)
    {
    }
    public DbSet<UserAcl> UserAcls { get; set; }

  }
}