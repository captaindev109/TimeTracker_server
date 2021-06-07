using System;
using System.Collections.Generic;

namespace TaskTypeApi.Models
{
  public class TaskType
  {
    public long id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public List<string> tags { get; set; }
    public long createdBy { get; set; }
    public long updatedBy { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum TaskTypeStatus
  {
    Active,
    Inactive,
    Archived,
  }
}