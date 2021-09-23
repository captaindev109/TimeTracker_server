using System;
using System.Collections.Generic;

namespace TimeTracker_server.Models
{
  public class ResourceType
  {
    public long id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public float hourlyRate { get; set; }
    public long company { get; set; }
    public long createdBy { get; set; }
    public long updatedBy { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum ResourceTypeStatus
  {
    Active,
    Inactive,
    Archived,
    Deleted
  }
}