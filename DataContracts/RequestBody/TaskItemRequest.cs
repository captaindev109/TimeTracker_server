using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class CreateTaskItemRequest
  {
    public TaskItem taskItem { get; set; }
    public long projectId { get; set; }
  }

  public class UpdateTaskItemRequest
  {
    public TaskItem taskItem { get; set; }
    public long projectId { get; set; }
  }

  public class GetBacklogTaskItemRequest
  {
    public long userId { get; set; }
    public long companyId { get; set; }
    public List<long> taskItemIds { get; set; }
  }
}
