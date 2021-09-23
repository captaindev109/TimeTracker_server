using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class CreateTaskTypeRequestRequest
  {
    public TaskType taskType { get; set; }
    public long companyId { get; set; }
    public List<string> tags { get; set; }
  }

  public class UpdateTaskTypeRequestRequest
  {
    public TaskType taskType { get; set; }
    public long companyId { get; set; }
    public List<string> tags { get; set; }
  }
}
