using System;

namespace TimeTracker_server.Models
{
  public class Company
  {
    public long id { get; set; }
    public string name { get; set; }
    public string status { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }
}