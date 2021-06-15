using System;
using System.Collections.Generic;

namespace TaskItemApi.Models
{
  public class TaskItem
  {
    public long id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public List<string> tags { get; set; }
    public long project { get; set; }
    public long company { get; set; }
    public long start { get; set; }
    public long end { get; set; }
    public long duration { get; set; }
    public long createdBy { get; set; }
    public long updatedBy { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum TaskItemStatus
  {
    AutoActive,
    Active,
    Progress,
    Done,
  }
}