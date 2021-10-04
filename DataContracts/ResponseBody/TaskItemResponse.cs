using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class ProgressItemResponse
  {
    public TaskItem taskItem { get; set; }
    public TimeTable timeTable { get; set; }
    public Project project { get; set; }
  }
}
