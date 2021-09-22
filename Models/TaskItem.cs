using System;
using System.Collections.Generic;

namespace TimeTracker_server.Models
{
  public class TaskItem
  {
    public long id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public long createdBy { get; set; }
    public long updatedBy { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum TaskItemStatus
  {
    AutoActive,
    Active,
    InWork,
    Progress,
    Done,
  }
}