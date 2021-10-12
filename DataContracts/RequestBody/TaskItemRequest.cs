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

  public class SetBacklogTaskItemRequest
  {
    public long userId { get; set; }
    public long companyId { get; set; }
    public List<long> taskItemIds { get; set; }
  }

  public class StartTrackingTaskItemRequest
  {
    public long taskItemId { get; set; }
    public long start { get; set; }
    public long userId { get; set; }
    public long companyId { get; set; }
  }

  public class ActionTrackingTaskItemRequest
  {
    public long taskItemId { get; set; }
    public long timeTableId { get; set; }
    public long currentTime { get; set; }
  }

  public class GetCurrentProgressTaskItemRequest
  {
    public long companyId { get; set; }
    public long userId { get; set; }
  }

  public class GetToDoTaskItemRequest
  {
    public long companyId { get; set; }
    public long userId { get; set; }
  }

   public class GetProgressTaskItemRequest
  {
    public long userId { get; set; }
    public long companyId { get; set; }
    public long taskItemId { get; set; }
  }

  public class SaveCommentTaskItemRequest
  {
    public long timeTableId { get; set; }
    public string comment { get; set; }
  }
}
