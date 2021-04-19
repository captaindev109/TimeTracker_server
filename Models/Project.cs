using System;

namespace ProjectApi.Models
{
  public class Project
  {
    public long id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public string public_status { get; set; }
    public DateTime plan_start { get; set; }
    public DateTime plan_end { get; set; }
    public string teams { get; set; }
    public string project_manager { get; set; }
    public string project_manager_assistant { get; set; }
    public long company { get; set; }
    public long created_by { get; set; }
    public long updated_by { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum ProjectStatus
  {
    Active,
    Locked,
    AwaitEmailConfirmation
  }
}