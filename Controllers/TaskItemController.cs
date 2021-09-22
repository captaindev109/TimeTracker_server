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

      var oldAcl = await _context.UserAcls.Where(x => x.sourceType == "taskItem" && x.role == "created_in" && x.objectId == projectId && x.objectType == "project").FirstOrDefaultAsync();
      _context.UserAcls.Remove(oldAcl);

      var userAcl = new UserAcl();
      userAcl.sourceId = taskItem.id;
      userAcl.sourceType = "taskItem";
      userAcl.role = "created_in";
      userAcl.objectId = projectId;
      userAcl.objectType = "project";
      userAcl.create_timestamp = DateTime.UtcNow;
      userAcl.update_timestamp = DateTime.UtcNow;
      _context.UserAcls.Add(userAcl);

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
  }
}
