using System;

namespace UserAclApi.Models
{
  public class UserAcl
  {
    public long id { get; set; }
    public long userId { get; set; }
    public UserRole role { get; set; }
    public long objectId { get; set; }
    public ObjectType objectType { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum UserRole
  {
    company_admin,
    controller,
    project_manager,
    assist_project_manager,
    team_lead,
    worker,
  }

  public enum ObjectType
  {
    company,
    project,
    team,
  }
}