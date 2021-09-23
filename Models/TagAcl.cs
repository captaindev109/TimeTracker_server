using System;

namespace TimeTracker_server.Models
{
  public class TagAcl
  {
    public long id { get; set; }
    public long tagId { get; set; }
    public long objectId { get; set; }
    public string objectType { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }
}