using System;

namespace TimeTracker_server.Models
{
  public class Tag
  {
    public long id { get; set; }
    public string name { get; set; }
    public long companyId { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }
}