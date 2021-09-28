using System;

namespace TimeTracker_server.Models
{
  public class TimeTable
  {
    public long id { get; set; }
    public long taskItem { get; set; }
    public string description { get; set; }
    public DateTime date { get; set; }
    public long start { get; set; }
    public long end { get; set; }
    public long pauseDuration { get; set; }
    public long pauseStart { get; set; }
  }
}