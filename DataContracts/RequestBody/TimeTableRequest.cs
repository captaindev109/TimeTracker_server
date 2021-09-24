using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class TimeTableRequest
  {
    public List<long> taskItems { get; set; }
    public int currentWeek { get; set; }
  }
}
