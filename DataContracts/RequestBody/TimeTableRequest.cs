using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class TimeTableRequest
  {
    public long userId { get; set; }
    public long companyId { get; set; }
    public int currentWeek { get; set; }
  }

  public class GetTaskItemTimeTable
  {
    public long userId { get; set; }
    public long companyId { get; set; }
    public long taskItemId { get; set; }
  }

  public class ManualLog
  {
    public List<DateTime> dates { get; set; }
    public long start { get; set; }
    public long end { get; set; }
    public string description { get; set; }
    public long taskItemId { get; set; }
    public long userId { get; set; }
    public long companyId { get; set; }
  }
}
