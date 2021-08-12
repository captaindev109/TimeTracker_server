using System;

namespace UserApi.Models
{
  public class User
  {
    public long id { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string avatar { get; set; }
    public string role { get; set; }
    public string status { get; set; }
    public bool isVerified { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum UserStatus
  {
    Active,
    Locked,
    Archived,
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

}