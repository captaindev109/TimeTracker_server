using System;

namespace TaskItemApi.Models
{
  public class TaskItem
  {
    public long id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public string position { get; set; }
    public string tag { get; set; }
    public long project { get; set; }
    public long company { get; set; }
    public long createdBy { get; set; }
    public long updatedBy { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum TaskItemPosition
  {
    Active,
    Inactive,
    Archived,
    Deleted
  }

   public enum TaskItemStatus
  {
    Active,
    Inactive,
    Archived,
  }
}