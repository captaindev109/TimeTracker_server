using System;

namespace UserAclApi.Models
{
  public class UserAcl
  {
    public long id { get; set; }
    public long sourceId { get; set; }
    public string sourceType { get; set; }
    public string role { get; set; }
    public long objectId { get; set; }
    public string objectType { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum UserRoleEnum
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