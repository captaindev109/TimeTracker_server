using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class WeekTimeTableResponse
  {
    public long taskItemId { get; set; }
    public List<TimeTable> weekLogs { get; set; }
  }
}
