using System;
using System.Collections.Generic;

namespace TimeTracker_server.Models
{
  public class Team
  {
    public long id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public long createdBy { get; set; }
    public long updatedBy { get; set; }
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