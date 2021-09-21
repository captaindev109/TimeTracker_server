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
  public class ProjectController : ControllerBase
  {
    private readonly MyDbContext _context;

    public ProjectController(MyDbContext context)
    {
      _context = context;
    }

    // GET: api/Project
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
      return await _context.Projects.ToListAsync();
    }

    // POST: api/Project/with-roles
    [HttpPost("with-roles")]
    public async Task<ActionResult<IEnumerable<ProjectWithRoleResponse>>> GetProjectsWithRoles(DataWithRolesRequest request)
    {
      var companyId = request.companyId;
      var userId = request.userId;
      List<string> roles = request.userRoles;

      var editableProjectIds = new List<long>();

      var projectIds = new List<long>();
      if (roles.Contains("company_admin") || roles.Contains("company_controller"))
      {
        var tmpProjectIds = await _context.UserAcls.Where(x => x.sourceType == "project" && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
        projectIds.AddRange(tmpProjectIds);
      }
      else if (roles.Contains("project_manager") || roles.Contains("project_assistant"))
      {
        var tmpProjectIds = await _context.UserAcls.Where(x => (x.role == "project_manager" || x.role == "project_assistant") && x.sourceType == "user" && x.sourceId == userId && x.objectType == "project").Select(x => x.objectId).ToListAsync();
        projectIds.AddRange(tmpProjectIds);
        editableProjectIds = tmpProjectIds;
      }
      else if (roles.Contains("team_lead") || roles.Contains("worker"))
      {
        var teamIds = await _context.UserAcls.Where(x => (x.role == "team_lead" || x.role == "worker") && x.sourceType == "user" && x.sourceId == userId && x.objectType == "team").Select(x => x.objectId).ToListAsync();
        var tmpProjectIds = await _context.UserAcls.Where(x => x.sourceType == "project" && x.role == "assigned_in" && x.objectType == "team" && teamIds.Contains(x.objectId)).Select(x => x.sourceId).ToListAsync();
        projectIds.AddRange(tmpProjectIds);
      }

      var allProjects = await _context.UserAcls.Where(x => x.sourceType == "project" && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
      var companyPublicProjectIds = await _context.Projects.Where(x => x.publicStatus == "Company_Search" && allProjects.Contains(x.id)).Select(x => x.id).ToListAsync();

      foreach (var id in companyPublicProjectIds)
      {
        if (!projectIds.Contains(id))
        {
          projectIds.Add(id);
        }
      } // public search 

      var projects = await _context.Projects.Where(x => projectIds.Contains(x.id)).ToListAsync();
      var resProjects = new List<ProjectWithRoleResponse>();

      foreach (var item in projects)
      {
        var isEditable = false;
        if (editableProjectIds.Contains(item.id))
        {
          isEditable = true;
        }
        var projectItem = new ProjectWithRoleResponse();
        projectItem.project = item;
        projectItem.isEditable = isEditable;

        var projectMangerAcls = await _context.UserAcls.Where(x => x.sourceType == "user" && x.role == "project_manager" && x.objectId == item.id && x.objectType == "project").Select(x => x.sourceId).ToListAsync();
        projectItem.projectManagers = await _context.Users.Where(x => projectMangerAcls.Contains(x.id)).ToListAsync();

        var projectMangerAssistantAcls = await _context.UserAcls.Where(x => x.sourceType == "user" && x.role == "project_assistant" && x.objectId == item.id && x.objectType == "project").Select(x => x.sourceId).ToListAsync();
        projectItem.projectManagerAssistants = await _context.Users.Where(x => projectMangerAssistantAcls.Contains(x.id)).ToListAsync();

        var teamAcls = await _context.UserAcls.Where(x => x.sourceType == "project" && x.role == "assigned_in" && x.sourceId == item.id && x.objectType == "team").Select(x => x.objectId).ToListAsync();
        projectItem.teams = await _context.Teams.Where(x => teamAcls.Contains(x.id)).ToListAsync();

        resProjects.Add(projectItem);
      }

      return resProjects;
    }

    // GET: api/Project/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(long id)
    {
      var project = await _context.Projects.FindAsync(id);

      if (project == null)
      {
        return NotFound();
      }

      return project;
    }

    // PUT: api/Project/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProject(long id, UpdateProjectRequest request)
    {
      var project = request.project;
      var projectManagers = request.projectManagers;
      var projectManagerAssistants = request.projectManagerAssistants;
      var teams = request.teams;

      if (id != project.id)
      {
        return BadRequest();
      }

      _context.Entry(project).State = EntityState.Modified;

      var oldAcls = await _context.UserAcls.Where(x =>
      (x.sourceType == "user" && x.role == "project_manager" && x.objectId == project.id && x.objectType == "project")
    || (x.sourceType == "user" && x.role == "project_assistant" && x.objectId == project.id && x.objectType == "project")
    || (x.sourceType == "project" && x.role == "assigned_in" && x.sourceId == project.id && x.objectType == "team")
      ).ToListAsync();

      _context.UserAcls.RemoveRange(oldAcls);

      foreach (var userId in projectManagers)
      {
        var newAcl = new UserAcl();
        newAcl.sourceId = userId;
        newAcl.sourceType = "user";
        newAcl.role = "project_manager";
        newAcl.objectId = project.id;
        newAcl.objectType = "project";
        newAcl.create_timestamp = DateTime.UtcNow;
        newAcl.update_timestamp = DateTime.UtcNow;

        _context.UserAcls.Add(newAcl);
      }

      foreach (var userId in projectManagerAssistants)
      {
        var newAcl = new UserAcl();
        newAcl.sourceId = userId;
        newAcl.sourceType = "user";
        newAcl.role = "project_assistant";
        newAcl.objectId = project.id;
        newAcl.objectType = "project";
        newAcl.create_timestamp = DateTime.UtcNow;
        newAcl.update_timestamp = DateTime.UtcNow;

        _context.UserAcls.Add(newAcl);
      }

      foreach (var teamId in teams)
      {
        var newAcl = new UserAcl();
        newAcl.sourceId = project.id;
        newAcl.sourceType = "project";
        newAcl.role = "assigned_in";
        newAcl.objectId = teamId;
        newAcl.objectType = "team";
        newAcl.create_timestamp = DateTime.UtcNow;
        newAcl.update_timestamp = DateTime.UtcNow;

        _context.UserAcls.Add(newAcl);
      }

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ProjectExists(id))
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

    // POST: api/Project
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Project>> PostProject(CreateProjectRequest request)
    {
      var companyId = request.companyId;
      var projectManagers = request.projectManagers;
      var projectManagerAssistants = request.projectManagerAssistants;
      var teams = request.teams;

      var project = request.project;
      project.create_timestamp = DateTime.UtcNow;
      project.update_timestamp = DateTime.UtcNow;
      _context.Projects.Add(project);
      await _context.SaveChangesAsync();

      var createdProjectId = project.id;

      var userAcl = new UserAcl();
      userAcl.sourceId = createdProjectId;
      userAcl.sourceType = "project";
      userAcl.role = "created_in";
      userAcl.objectId = companyId;
      userAcl.objectType = "company";
      userAcl.create_timestamp = DateTime.UtcNow;
      userAcl.update_timestamp = DateTime.UtcNow;
      _context.UserAcls.Add(userAcl);

      foreach (var userId in projectManagers)
      {
        var newAcl = new UserAcl();
        newAcl.sourceId = userId;
        newAcl.sourceType = "user";
        newAcl.role = "project_manager";
        newAcl.objectId = createdProjectId;
        newAcl.objectType = "project";
        newAcl.create_timestamp = DateTime.UtcNow;
        newAcl.update_timestamp = DateTime.UtcNow;

        _context.UserAcls.Add(newAcl);
      }

      foreach (var userId in projectManagerAssistants)
      {
        var newAcl = new UserAcl();
        newAcl.sourceId = userId;
        newAcl.sourceType = "user";
        newAcl.role = "project_assistant";
        newAcl.objectId = createdProjectId;
        newAcl.objectType = "project";
        newAcl.create_timestamp = DateTime.UtcNow;
        newAcl.update_timestamp = DateTime.UtcNow;

        _context.UserAcls.Add(newAcl);
      }

      foreach (var teamId in teams)
      {
        var newAcl = new UserAcl();
        newAcl.sourceId = createdProjectId;
        newAcl.sourceType = "project";
        newAcl.role = "assigned_in";
        newAcl.objectId = teamId;
        newAcl.objectType = "team";
        newAcl.create_timestamp = DateTime.UtcNow;
        newAcl.update_timestamp = DateTime.UtcNow;

        _context.UserAcls.Add(newAcl);
      }

      await _context.SaveChangesAsync();
      return project;
    }

    // DELETE: api/Project/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(long id)
    {
      var project = await _context.Projects.FindAsync(id);
      if (project == null)
      {
        return NotFound();
      }

      _context.Projects.Remove(project);

      var oldAcls = await _context.UserAcls.Where(x =>
       (x.objectType == "company" && x.role == "created_in" && x.sourceId == project.id && x.sourceType == "project")
    || (x.sourceType == "user" && (x.role == "project_manager" || x.role == "project_assistant") && x.objectId == project.id && x.objectType == "project")
    || (x.sourceType == "project" && x.role == "assigned_in" && x.sourceId == project.id && x.objectType == "team")
     ).ToListAsync();

      _context.UserAcls.RemoveRange(oldAcls);

      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool ProjectExists(long id)
    {
      return _context.Projects.Any(e => e.id == id);
    }
  }
}
