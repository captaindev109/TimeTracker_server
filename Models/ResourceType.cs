using System;

namespace ResourceTypeApi.Models
{
  public class ResourceType
  {
    public Guid id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public float hourly_rate { get; set; }
    public Guid company { get; set; }
    public Guid created_by { get; set; }
    public Guid updated_by { get; set; }
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