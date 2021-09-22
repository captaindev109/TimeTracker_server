using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class TaskItemWithRoleResponse
  {
    public TaskItem taskItem { get; set; }
    public bool isEditable { get; set; }
    public Project project { get; set; }
  }
}
