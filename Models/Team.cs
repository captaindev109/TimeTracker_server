using System;

namespace TeamApi.Models
{
  public class Team
  {
    public Guid id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public Guid company { get; set; }
    public long created_by { get; set; }
    public long updated_by { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum TeamStatus
  {
    Active,
    Inactive,
    Archived,
    Deleted
  }
}