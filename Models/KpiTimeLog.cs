using System;

namespace TimeTracker_server.Models
{
  public class KpiTimeLog
  {
    public long objectId { get; set; }
    public string objectType { get; set; }
    public long companyId { get; set; }
    public long userId { get; set; }
    public long value { get; set; }
  }
}