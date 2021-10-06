using System;

namespace TimeTracker_server.Models
{
  public class TaskItemAcl
  {
    public long id { get; set; }
    public long taskItemId { get; set; }
    public long userId { get; set; }
    public long companyId { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }
}