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
      List<string> roles = request.userRoles;

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

      var oldAcls = _context.TaskItemAcls.Where(x => x.userId == userId && x.companyId == companyId);
      var removeAcls = await oldAcls.Where(x => x.timeTableId == -1).ToListAsync();
      _context.TaskItemAcls.RemoveRange(removeAcls);
      await _context.SaveChangesAsync();

      var needToUpdateAcls = await oldAcls.Where(x => x.status == "Active").ToListAsync();
      var progressId = await oldAcls.Where(x => x.status == "Progress").Select(x => x.taskItemId).FirstOrDefaultAsync();
      foreach (var activeAcl in needToUpdateAcls)
      {
        activeAcl.status = "Draft";
        activeAcl.update_timestamp = DateTime.UtcNow;
        _context.Entry(activeAcl).State = EntityState.Modified;
      }
      await _context.SaveChangesAsync();

      foreach (var taskItemId in taskItemIds)
      {
        if (progressId == taskItemId)
        {
          continue;
        }
        var taskItemAcl = new TaskItemAcl();
        taskItemAcl.userId = userId;
        taskItemAcl.companyId = companyId;
        taskItemAcl.taskItemId = taskItemId;
        taskItemAcl.timeTableId = -1;
        taskItemAcl.status = "Active";
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
      var backlogItemIds = await _context.TaskItemAcls.Where(x => x.userId == userId && x.companyId == companyId && x.status == "Active").Select(x => x.taskItemId).ToListAsync();

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
      _context.TimeTables.Add(timeTable);
      await _context.SaveChangesAsync();

      var taskItemAcl = _context.TaskItemAcls.Where(x => x.taskItemId == taskItemId && x.companyId == companyId && x.userId == userId && x.status == "Active");

      var oldTaskItemAcl = await taskItemAcl.Where(x => x.timeTableId == -1).FirstOrDefaultAsync();
      if (oldTaskItemAcl != null)
      {
        oldTaskItemAcl.update_timestamp = DateTime.UtcNow;
        oldTaskItemAcl.status = "Progress";
        oldTaskItemAcl.timeTableId = timeTable.id;
        _context.Entry(oldTaskItemAcl).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
      }

      var draftTaskItemAcl = await taskItemAcl.Where(x => x.timeTableId != -1).FirstOrDefaultAsync();
      if (draftTaskItemAcl != null)
      {
        draftTaskItemAcl.update_timestamp = DateTime.UtcNow;
        draftTaskItemAcl.status = "Draft";
        _context.Entry(draftTaskItemAcl).State = EntityState.Modified;
        // await _context.SaveChangesAsync();

        var newAcl = new TaskItemAcl();
        newAcl.taskItemId = taskItemId;
        newAcl.userId = userId;
        newAcl.companyId = companyId;
        newAcl.update_timestamp = DateTime.UtcNow;
        newAcl.create_timestamp = DateTime.UtcNow;
        newAcl.status = "Progress";
        newAcl.timeTableId = timeTable.id;
        _context.TaskItemAcls.Add(newAcl);
        await _context.SaveChangesAsync();
      }

      return NoContent();
    }

    // POST: api/TaskItem/getProgressLog
    [HttpPost("getProgressLog")]
    public async Task<ActionResult<TimeTable>> getTaskItemProgressLog(GetProgressTaskItemRequest request)
    {
      var userId = request.userId;
      var companyId = request.companyId;
      var taskItemId = request.taskItemId;

      var progressTimeTableId = await _context.TaskItemAcls.Where(x => x.taskItemId == taskItemId && x.userId == userId && x.companyId == companyId && x.status == "Progress").Select(x => x.timeTableId).FirstOrDefaultAsync();
      var progressLog = await _context.TimeTables.Where(x => x.id == progressTimeTableId && x.end == -1).FirstOrDefaultAsync();

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

      _context.Entry(curTimeTable).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      var curTaskItemAcl = await _context.TaskItemAcls.Where(x => x.taskItemId == taskItemId && x.timeTableId == timeTableId).FirstOrDefaultAsync();
      curTaskItemAcl.status = "Active";
      curTaskItemAcl.update_timestamp = DateTime.UtcNow;
      _context.Entry(curTaskItemAcl).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      return NoContent();
    }

    // POST: api/TaskItem/getCurrentProgress
    [HttpPost("getCurrentProgressItem")]
    public async Task<ActionResult<ProgressItemResponse>> getCurrentProgressTaskItem(GetCurrentProgressTaskItemRequest request)
    {
      var companyId = request.companyId;
      var userId = request.userId;

      var progressItem = await _context.TaskItemAcls.Where(x => x.userId == userId && x.companyId == companyId && x.status == "Progress").FirstOrDefaultAsync();
      if (progressItem == null)
      {
        return NotFound();
      }
      var timeTable = await _context.TimeTables.FindAsync(progressItem.timeTableId);
      var taskItem = await _context.TaskItems.FindAsync(progressItem.taskItemId);

      var createdInProjectId = await _context.UserAcls.Where(x => x.sourceType == "taskItem" && x.role == "created_in" && x.sourceId == progressItem.taskItemId && x.objectType == "project").Select(x => x.objectId).FirstOrDefaultAsync();
      var project = await _context.Projects.FindAsync(createdInProjectId);

      var progressItemRes = new ProgressItemResponse();
      progressItemRes.taskItem = taskItem;
      progressItemRes.timeTable = timeTable;
      progressItemRes.project = project;

      return progressItemRes;
    }

  }
}
