using System;

namespace TimeTableApi.Models
{
  public class TimeTable
  {
    public long id { get; set; }
    public string task { get; set; }
    public string description { get; set; }
    public DateTime date { get; set; }
    public string start { get; set; }
    public string end { get; set; }
    public long project { get; set; }
  }
}