using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class TaskTypeResponse
  {
    public TaskType taskType { get; set; }
    public List<Tag> tags { get; set; }
  }
}
