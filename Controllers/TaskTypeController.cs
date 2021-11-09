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
  public class TaskTypeController : ControllerBase
  {
    private readonly MyDbContext _context;

    public TaskTypeController(MyDbContext context)
    {
      _context = context;
    }

    // GET: api/TaskType
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskType>>> GetTaskTypes()
    {
      return await _context.TaskTypes.ToListAsync();
    }

    // GET: api/TaskType/company/5
    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<IEnumerable<TaskTypeResponse>>> GetTaskTypesOfCompany(long companyId)
    {
      var taskTypeIds = await _context.UserAcls.Where(x => x.sourceType == "taskType" && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
      Console.WriteLine(taskTypeIds);
      var taskTypes = await _context.TaskTypes.Where(x => taskTypeIds.Contains(x.id)).ToListAsync();

      var resTaskTypes = new List<TaskTypeResponse>();
      foreach (var item in taskTypes)
      {
        var tagIds = await _context.TagAcls.Where(x => x.objectId == item.id && x.objectType == "taskType").Select(x => x.tagId).ToListAsync();
        var tags = await _context.Tags.Where(x => tagIds.Contains(x.id)).ToListAsync();
        var taskTypeItem = new TaskTypeResponse();
        taskTypeItem.taskType = item;
        taskTypeItem.tags = tags;
        resTaskTypes.Add(taskTypeItem);
      }

      return resTaskTypes;
    }

    // GET: api/TaskType/tags/company/5
    [HttpGet("tags/company/{companyId}")]
    public async Task<ActionResult<IEnumerable<Tag>>> GetTaskTypeTagsOfCompany(long companyId)
    {
      var tags = await _context.Tags.Where(x => x.companyId == companyId).ToListAsync();

      return tags;
    }

    // GET: api/TaskType/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskType>> GetTaskType(long id)
    {
      var taskType = await _context.TaskTypes.FindAsync(id);

      if (taskType == null)
      {
        return NotFound();
      }

      return taskType;
    }

    // PUT: api/TaskType/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTaskType(long id, UpdateTaskTypeRequestRequest request)
    {
      var taskType = request.taskType;
      var companyId = request.companyId;
      var tags = request.tags;

      if (id != taskType.id)
      {
        return BadRequest();
      }

      _context.Entry(taskType).State = EntityState.Modified;

      var tagsAcl = await _context.TagAcls.Where(x => x.objectId == id && x.objectType == "taskType").ToListAsync();
      _context.TagAcls.RemoveRange(tagsAcl);
      await _context.SaveChangesAsync();

      var tagsOfCompany = await _context.Tags.Where(x => x.companyId == companyId).ToListAsync();

      foreach (var tagName in tags)
      {
        var existTag = tagsOfCompany.FirstOrDefault(x => x.name == tagName);
        if (existTag == null)
        {
          var newTag = new Tag();
          newTag.name = tagName;
          newTag.companyId = companyId;
          newTag.create_timestamp = DateTime.UtcNow;
          newTag.update_timestamp = DateTime.UtcNow;
          _context.Tags.Add(newTag);
          await _context.SaveChangesAsync();

          var createdTagId = newTag.id;

          var tagAcl = new TagAcl();
          tagAcl.tagId = createdTagId;
          tagAcl.objectType = "taskType";
          tagAcl.objectId = taskType.id;
          tagAcl.create_timestamp = DateTime.UtcNow;
          tagAcl.update_timestamp = DateTime.UtcNow;
          _context.TagAcls.Add(tagAcl);
        }
        else
        {
          var tagAcl = new TagAcl();
          tagAcl.tagId = existTag.id;
          tagAcl.objectType = "taskType";
          tagAcl.objectId = taskType.id;
          tagAcl.create_timestamp = DateTime.UtcNow;
          tagAcl.update_timestamp = DateTime.UtcNow;
          _context.TagAcls.Add(tagAcl);
        }
      }

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!TaskTypeExists(id))
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

    // POST: api/TaskType
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TaskType>> PostTaskType(CreateTaskTypeRequestRequest request)
    {
      var taskType = request.taskType;
      taskType.create_timestamp = DateTime.UtcNow;
      taskType.update_timestamp = DateTime.UtcNow;
      var companyId = request.companyId;
      var tags = request.tags;

      _context.TaskTypes.Add(taskType);
      await _context.SaveChangesAsync();

      var createdTaskTypeId = taskType.id;

      var taskTypeAcl = new UserAcl();
      taskTypeAcl.sourceId = createdTaskTypeId;
      taskTypeAcl.sourceType = "taskType";
      taskTypeAcl.role = "created_in";
      taskTypeAcl.objectId = companyId;
      taskTypeAcl.objectType = "company";
      taskTypeAcl.companyId = companyId;
      taskTypeAcl.create_timestamp = DateTime.UtcNow;
      taskTypeAcl.update_timestamp = DateTime.UtcNow;
      _context.UserAcls.Add(taskTypeAcl);
      await _context.SaveChangesAsync();

      var tagsOfCompany = await _context.Tags.Where(x => x.companyId == companyId).ToListAsync();

      foreach (var tagName in tags)
      {
        var existTag = tagsOfCompany.FirstOrDefault(x => x.name == tagName);
        if (existTag == null)
        {
          var newTag = new Tag();
          newTag.name = tagName;
          newTag.companyId = companyId;
          newTag.create_timestamp = DateTime.UtcNow;
          newTag.update_timestamp = DateTime.UtcNow;
          _context.Tags.Add(newTag);
          await _context.SaveChangesAsync();

          var createdTagId = newTag.id;

          var tagAcl = new TagAcl();
          tagAcl.tagId = createdTagId;
          tagAcl.objectType = "taskType";
          tagAcl.objectId = createdTaskTypeId;
          tagAcl.create_timestamp = DateTime.UtcNow;
          tagAcl.update_timestamp = DateTime.UtcNow;
          _context.TagAcls.Add(tagAcl);
        }
        else
        {
          var tagAcl = new TagAcl();
          tagAcl.tagId = existTag.id;
          tagAcl.objectType = "taskType";
          tagAcl.objectId = createdTaskTypeId;
          tagAcl.create_timestamp = DateTime.UtcNow;
          tagAcl.update_timestamp = DateTime.UtcNow;
          _context.TagAcls.Add(tagAcl);
        }
      }
      await _context.SaveChangesAsync();

      return taskType;
    }

    // DELETE: api/TaskType/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTaskType(long id)
    {
      var taskType = await _context.TaskTypes.FindAsync(id);
      if (taskType == null)
      {
        return NotFound();
      }

      var taskTypeAcl = await _context.UserAcls.Where(x => x.sourceId == id && x.sourceType == "taskType" && x.role == "created_in").FirstOrDefaultAsync();
      _context.UserAcls.Remove(taskTypeAcl);

      var tagsAcl = await _context.TagAcls.Where(x => x.objectId == id && x.objectType == "taskType").ToListAsync();
      _context.TagAcls.RemoveRange(tagsAcl);

      _context.TaskTypes.Remove(taskType);

      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool TaskTypeExists(long id)
    {
      return _context.TaskTypes.Any(e => e.id == id);
    }
  }
}
