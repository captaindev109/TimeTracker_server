using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker_server.Models;
using TimeTracker_server.Data;
using DataContracts.RequestBody;

namespace TimeTracker_server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TaskItemController : ControllerBase
  {
    private readonly MyDbContext _context;

    public TaskItemController(MyDbContext context)
    {
      _context = context;
    }

    // GET: api/TaskItem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems()
    {
      return await _context.TaskItems.ToListAsync();
    }

    // GET: api/TaskItem/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTaskItem(long id)
    {
      var taskItem = await _context.TaskItems.FindAsync(id);

      if (taskItem == null)
      {
        return NotFound();
      }

      return taskItem;
    }

    // PUT: api/TaskItem/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTaskItem(long id, UpdateTaskItemRequest request)
    {
      var projectId = request.projectId;

      var taskItem = request.taskItem;
      taskItem.update_timestamp = DateTime.UtcNow;


      if (id != taskItem.id)
      {
        return BadRequest();
      }
      _context.Entry(taskItem).State = EntityState.Modified;

      var taskItmesAlcs = await _context.TaskItemAcls.Where(x => x.taskItemId == id).ToListAsync();
      _context.TaskItemAcls.RemoveRange(taskItmesAlcs);

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!TaskItemExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/TaskItem
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TaskItem>> PostTaskItem(CreateTaskItemRequest request)
    {
      var projectId = request.projectId;
      var companyId = request.companyId;

      var taskItem = request.taskItem;
      taskItem.create_timestamp = DateTime.UtcNow;
      taskItem.update_timestamp = DateTime.UtcNow;
      _context.TaskItems.Add(taskItem);
      await _context.SaveChangesAsync();

      var createdTaskItemId = taskItem.id;

      var userAcl = new UserAcl();
      userAcl.sourceId = createdTaskItemId;
      userAcl.sourceType = "taskItem";
      userAcl.role = "created_in";
      userAcl.objectId = projectId;
      userAcl.objectType = "project";
      userAcl.companyId = companyId;
      userAcl.create_timestamp = DateTime.UtcNow;
      userAcl.update_timestamp = DateTime.UtcNow;
      _context.UserAcls.Add(userAcl);
      await _context.SaveChangesAsync();

      return taskItem;
    }

    // DELETE: api/TaskItem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTaskItem(long id)
    {
      var taskItem = await _context.TaskItems.FindAsync(id);
      if (taskItem == null)
      {
        return NotFound();
      }
      var oldAcl = await _context.UserAcls.Where(x => x.sourceType == "taskItem" && x.role == "created_in" && x.sourceId == id && x.objectType == "project").FirstOrDefaultAsync();
      _context.UserAcls.Remove(oldAcl);

      var oldTaskItemAcl = await _context.TaskItemAcls.Where(x => x.taskItemId == id).ToListAsync();
      _context.TaskItemAcls.RemoveRange(oldTaskItemAcl);

      _context.TaskItems.Remove(taskItem);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool TaskItemExists(long id)
    {
      return _context.TaskItems.Any(e => e.id == id);
    }

    // POST: api/TaskItem/with-roles
    [HttpPost("with-roles")]
    public async Task<ActionResult<IEnumerable<TaskItemWithRoleResponse>>> GetProjectsWithRoles(DataWithRolesRequest request)
    {
      var companyId = request.companyId;
      var userId = request.userId;
      List<string> roles = getRoles(companyId, userId).Result;

      var editableTaskItemIds = new List<long>();

      var taskItemIds = new List<long>();
      if (roles.Contains("company_admin") || roles.Contains("company_controller"))
      {
        var tmpProjectIds = await _context.UserAcls.Where(x => x.sourceType == "project" && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
        var tmpTaskIds = await _context.UserAcls.Where(x => x.sourceType == "taskItem" && x.role == "created_in" && tmpProjectIds.Contains(x.objectId) && x.objectType == "project").Select(x => x.sourceId).ToListAsync();
        taskItemIds.AddRange(tmpTaskIds);
        editableTaskItemIds.AddRange(tmpTaskIds);
      }
      else if (roles.Contains("project_manager") || roles.Contains("project_assistant"))
      {
        var tmpProjectIds = await _context.UserAcls.Where(x => (x.role == "project_manager" || x.role == "project_assistant") && x.sourceType == "user" && x.sourceId == userId && x.objectType == "project").Select(x => x.objectId).ToListAsync();
        var tmpTaskIds = await _context.UserAcls.Where(x => x.sourceType == "taskItem" && x.role == "created_in" && tmpProjectIds.Contains(x.objectId) && x.objectType == "project").Select(x => x.sourceId).ToListAsync();
        taskItemIds.AddRange(tmpTaskIds);
        editableTaskItemIds.AddRange(tmpTaskIds);
      }
      else if (roles.Contains("team_lead") || roles.Contains("worker"))
      {
        var teamIds = await _context.UserAcls.Where(x => (x.role == "team_lead" || x.role == "worker") && x.sourceType == "user" && x.sourceId == userId && x.objectType == "team").Select(x => x.objectId).ToListAsync();
        var tmpProjectIds = await _context.UserAcls.Where(x => x.sourceType == "project" && x.role == "assigned_in" && x.objectType == "team" && teamIds.Contains(x.objectId)).Select(x => x.sourceId).ToListAsync();
        var tmpTaskIds = await _context.UserAcls.Where(x => x.sourceType == "taskItem" && x.role == "created_in" && tmpProjectIds.Contains(x.objectId) && x.objectType == "project").Select(x => x.sourceId).ToListAsync();
        taskItemIds.AddRange(tmpTaskIds);
      }

      var taskItems = await _context.TaskItems.Where(x => taskItemIds.Contains(x.id)).ToListAsync();
      var resTaskItems = new List<TaskItemWithRoleResponse>();

      foreach (var item in taskItems)
      {
        var isEditable = false;
        if (editableTaskItemIds.Contains(item.id))
        {
          isEditable = true;
        }
        var taskItem = new TaskItemWithRoleResponse();
        taskItem.taskItem = item;
        taskItem.isEditable = isEditable;

        var createdInProject = await _context.UserAcls.Where(x => x.sourceType == "taskItem" && x.role == "created_in" && x.sourceId == item.id && x.objectType == "project").Select(x => x.objectId).FirstOrDefaultAsync();
        taskItem.project = await _context.Projects.Where(x => createdInProject == x.id).FirstOrDefaultAsync();

        resTaskItems.Add(taskItem);
      }

      return resTaskItems;
    }

    // POST: api/TaskItem/setBacklog
    [HttpPost("setBacklog")]
    public async Task<ActionResult<TaskItem>> setBacklogTaskItem(SetBacklogTaskItemRequest request)
    {
      var userId = request.userId;
      var companyId = request.companyId;
      var taskItemIds = request.taskItemIds;

      var oldAcls = await _context.TaskItemAcls.Where(x => x.userId == userId && x.companyId == companyId).ToListAsync();
      _context.TaskItemAcls.RemoveRange(oldAcls);
      await _context.SaveChangesAsync();

      foreach (var taskItemId in taskItemIds)
      {
        var taskItemAcl = new TaskItemAcl();
        taskItemAcl.userId = userId;
        taskItemAcl.companyId = companyId;
        taskItemAcl.taskItemId = taskItemId;
        taskItemAcl.create_timestamp = DateTime.UtcNow;
        taskItemAcl.update_timestamp = DateTime.UtcNow;
        _context.TaskItemAcls.Add(taskItemAcl);
      }

      await _context.SaveChangesAsync();

      return NoContent();
    }

    // GET: api/TaskItem/getBacklog/userId/5/companyId/4
    [HttpGet("getBacklog/userId/{userId}/companyId/{companyId}")]
    public async Task<IEnumerable<long>> getBacklogTaskItem(long userId, long companyId)
    {
      var backlogItemIds = await _context.TaskItemAcls.Where(x => x.userId == userId && x.companyId == companyId).Select(x => x.taskItemId).ToListAsync();

      return backlogItemIds;
    }

    // POST: api/TaskItem/startTracking
    [HttpPost("startTracking")]
    public async Task<ActionResult<TaskItem>> startTrackingTaskItem(StartTrackingTaskItemRequest request)
    {
      var taskItemId = request.taskItemId;
      var start = request.start;
      var userId = request.userId;
      var companyId = request.companyId;

      var timeTable = new TimeTable();
      timeTable.date = DateTime.UtcNow;
      timeTable.start = start;
      timeTable.end = -1;
      timeTable.pauseStart = -1;
      timeTable.companyId = companyId;
      timeTable.userId = userId;
      timeTable.taskItemId = taskItemId;
      timeTable.status = "Progress";
      _context.TimeTables.Add(timeTable);

      await _context.SaveChangesAsync();

      return NoContent();
    }

    // POST: api/TaskItem/getProgressLog
    [HttpPost("getProgressLog")]
    public async Task<ActionResult<TimeTable>> getTaskItemProgressLog(GetProgressTaskItemRequest request)
    {
      var userId = request.userId;
      var companyId = request.companyId;
      var taskItemId = request.taskItemId;

      var progressLog = await _context.TimeTables.Where(x => x.taskItemId == taskItemId && x.userId == userId && x.companyId == companyId && x.status == "Progress").FirstOrDefaultAsync();

      return progressLog;
    }

    // POST: api/TaskItem/pauseTracking
    [HttpPost("pauseTracking")]
    public async Task<ActionResult<TaskItem>> pauseTrackingTaskItem(ActionTrackingTaskItemRequest request)
    {
      // var taskItemId = request.taskItemId;
      var timeTableId = request.timeTableId;
      var currentTime = request.currentTime;

      var curTimeTable = await _context.TimeTables.FindAsync(timeTableId);
      curTimeTable.pauseStart = currentTime;

      _context.Entry(curTimeTable).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      return NoContent();
    }

    // POST: api/TaskItem/restartTracking
    [HttpPost("restartTracking")]
    public async Task<ActionResult<TaskItem>> restartTrackingTaskItem(ActionTrackingTaskItemRequest request)
    {
      // var taskItemId = request.taskItemId;
      var timeTableId = request.timeTableId;
      var currentTime = request.currentTime;

      var curTimeTable = await _context.TimeTables.FindAsync(timeTableId);
      curTimeTable.pauseDuration += (currentTime - curTimeTable.pauseStart);
      curTimeTable.pauseStart = -1;

      _context.Entry(curTimeTable).State = EntityState.Modified;
      await _context.SaveChangesAsync();
      return NoContent();
    }

    // POST: api/TaskItem/stopTracking
    [HttpPost("stopTracking")]
    public async Task<ActionResult<TaskItem>> stopTrackingTaskItem(ActionTrackingTaskItemRequest request)
    {
      var taskItemId = request.taskItemId;
      var timeTableId = request.timeTableId;
      var currentTime = request.currentTime;

      var curTimeTable = await _context.TimeTables.FindAsync(timeTableId);
      if (curTimeTable.pauseStart != -1)
      {
        curTimeTable.pauseDuration += (currentTime - curTimeTable.pauseStart);
        curTimeTable.pauseStart = -1;
      }
      curTimeTable.end = currentTime;
      curTimeTable.status = "Active";

      _context.Entry(curTimeTable).State = EntityState.Modified;

      await _context.SaveChangesAsync();

      return NoContent();
    }

    // POST: api/TaskItem/getCurrentProgress
    [HttpPost("getCurrentProgressItem")]
    public async Task<ActionResult<ProgressItemResponse>> getCurrentProgressTaskItem(GetCurrentProgressTaskItemRequest request)
    {
      var companyId = request.companyId;
      var userId = request.userId;

      var progressTimeTable = await _context.TimeTables.Where(x => x.userId == userId && x.companyId == companyId && x.status == "Progress").FirstOrDefaultAsync();
      if (progressTimeTable == null)
      {
        return NotFound();
      }
      var taskItem = await _context.TaskItems.FindAsync(progressTimeTable.taskItemId);

      var createdInProjectId = await _context.UserAcls.Where(x => x.sourceType == "taskItem" && x.role == "created_in" && x.sourceId == progressTimeTable.taskItemId && x.objectType == "project").Select(x => x.objectId).FirstOrDefaultAsync();
      var project = await _context.Projects.FindAsync(createdInProjectId);

      var progressItemRes = new ProgressItemResponse();
      progressItemRes.taskItem = taskItem;
      progressItemRes.timeTable = progressTimeTable;
      progressItemRes.project = project;

      return progressItemRes;
    }

    // POST: api/TaskItem/saveComment
    [HttpPost("saveComment")]
    public async Task<ActionResult<TaskItem>> saveTaskItemComment(SaveCommentTaskItemRequest request)
    {
      var timeTableId = request.timeTableId;
      var comment = request.comment;

      var timeTableItem = await _context.TimeTables.FindAsync(timeTableId);

      if (timeTableItem == null)
      {
        return NotFound();
      }
      timeTableItem.description = comment;
      _context.Entry(timeTableItem).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private async Task<List<string>> getRoles(long companyId, long userId)
    {
      var roleList = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && !x.role.Contains("member") && x.objectType == "company" && x.objectId == companyId).Select(x => x.role).Distinct().ToListAsync();

      return roleList;
    }

    // POST: api/TaskItem/addManualLog
    [HttpPost("addManualLog")]
    public async Task<ActionResult<TimeTable>> addManualLog(ManualLog request)
    {
      List<DateTime> dates = request.dates;
      long start = request.start;
      long end = request.end;
      long pauseDuration = request.pauseDuration;
      string description = request.description;
      long taskItemId = request.taskItemId;
      long userId = request.userId;
      long companyId = request.companyId;

      foreach (var date in dates)
      {
        var timeTable = new TimeTable();
        timeTable.date = date;
        timeTable.start = start;
        timeTable.end = end;
        timeTable.taskItemId = taskItemId;
        timeTable.userId = userId;
        timeTable.description = description;
        timeTable.companyId = companyId;
        timeTable.status = "Active";
        timeTable.pauseStart = -1;
        timeTable.pauseDuration = pauseDuration;

        _context.TimeTables.Add(timeTable);
      }

      await _context.SaveChangesAsync();

      return NoContent();
    }

    // POST: api/TaskItem/updateLog
    [HttpPost("updateLog")]
    public async Task<ActionResult<TimeTable>> updateLog(UpdateLog request)
    {
      long timeTableId = request.id;
      DateTime date = request.date;
      long start = request.start;
      long end = request.end;
      long pauseDuration = request.pauseDuration;
      string description = request.description;

      var timeTable = await _context.TimeTables.FindAsync(timeTableId);

      if (timeTable == null)
      {
        return NotFound();
      }

      timeTable.date = date;
      timeTable.start = start;
      timeTable.end = end;
      timeTable.description = description;
      timeTable.pauseDuration = pauseDuration;

      _context.Entry(timeTable).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      return NoContent();
    }
  
    // POST: api/TaskItem/getLog
    [HttpPost("getLog")]
    public async Task<ActionResult<IEnumerable<TimeTable>>> getTaskItemTrackedLog(CommonRequest request)
    {
      var userId = request.userId;
      var companyId = request.companyId;

      var timeTables = await _context.TimeTables.Where(x => x.userId == userId && x.companyId == companyId && x.status != "Progress").ToListAsync();

      return timeTables;
    }

  }
}
