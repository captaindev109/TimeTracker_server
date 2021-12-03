using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker_server.Models;
using TimeTracker_server.Data;
using TimeTracker_server.Repositories;
using DataContracts.RequestBody;

namespace TimeTracker_server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProjectController : ControllerBase
  {
    private readonly MyDbContext _context;
    private CommonRepository commonRepository;
    public ProjectController(MyDbContext context)
    {
      _context = context;
      commonRepository = new CommonRepository(_context);
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

      List<string> roles = commonRepository.getRoles(companyId, userId).Result;

      var editableProjectIds = new List<long>();

      var projectIds = new List<long>();
      if (roles.Contains("company_admin") || roles.Contains("company_controller"))
      {
        var tmpProjectIds = await _context.UserAcls.Where(x => x.sourceType == "project" && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
        projectIds.AddRange(tmpProjectIds);
        editableProjectIds.AddRange(tmpProjectIds);
      }
      else if (roles.Contains("project_manager") || roles.Contains("project_assistant"))
      {
        var tmpProjectIds = await _context.UserAcls.Where(x => (x.role == "project_manager" || x.role == "project_assistant") && x.sourceType == "user" && x.sourceId == userId && x.objectType == "project").Select(x => x.objectId).ToListAsync();
        projectIds.AddRange(tmpProjectIds);
        editableProjectIds.AddRange(tmpProjectIds);
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

        var tagIds = await _context.TagAcls.Where(x => x.objectId == item.id && x.objectType == "project").Select(x => x.tagId).ToListAsync();
        var tags = await _context.Tags.Where(x => tagIds.Contains(x.id)).ToListAsync();
        projectItem.tags = tags;

        var kpis = await commonRepository.getKpi(item.id, "project", companyId, userId);

        projectItem.kpis = kpis;

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
      var companyId = request.companyId;
      project.update_timestamp = DateTime.UtcNow;
      var projectManagers = request.projectManagers;
      var projectManagerAssistants = request.projectManagerAssistants;
      var teams = request.teams;
      var tags = request.tags;

      if (!ProjectExists(id))
      {
        return NotFound();
      }

      _context.Entry(project).State = EntityState.Modified;

      // ---------user acl management---------

      var oldAcls = await _context.UserAcls.Where(x =>
      (x.sourceType == "user" && x.role == "project_manager" && x.objectId == project.id && x.objectType == "project")
    || (x.sourceType == "user" && x.role == "project_assistant" && x.objectId == project.id && x.objectType == "project")
    || (x.sourceType == "project" && x.role == "assigned_in" && x.sourceId == project.id && x.objectType == "team")
      ).ToListAsync();

      var projectMangerOldAcls = oldAcls.Where(x => x.role == "project_manager").Select(x => x.sourceId).ToList();
      var removedProjectManagers = projectMangerOldAcls.Except(projectManagers);
      var removedProjectManagerAcls = oldAcls.Where(x => x.role == "project_manager" && removedProjectManagers.Contains(x.sourceId)).ToList();
      _context.UserAcls.RemoveRange(removedProjectManagerAcls);

      var newProjectManagers = projectManagers.Except(projectMangerOldAcls);
      foreach (var userId in newProjectManagers)
      {
        commonRepository.addUserAcl(userId, "user", "project_manager", project.id, "project", companyId);
      }

      var projectAssistantOldAcls = oldAcls.Where(x => x.role == "project_assistant").Select(x => x.sourceId).ToList();
      var removedProjectAssistants = projectAssistantOldAcls.Except(projectManagerAssistants);
      var removedProjectAssistantAcls = oldAcls.Where(x => x.role == "project_assistant" && removedProjectAssistants.Contains(x.sourceId)).ToList();
      _context.UserAcls.RemoveRange(removedProjectAssistantAcls);

      var newProjectAssistants = projectManagerAssistants.Except(projectAssistantOldAcls);

      foreach (var userId in newProjectAssistants)
      {
        commonRepository.addUserAcl(userId, "user", "project_assistant", project.id, "project", companyId);
      }

      var teamOldAcls = oldAcls.Where(x => x.role == "assigned_in").Select(x => x.objectId).ToList();
      var removedTeams = teamOldAcls.Except(teams);
      var removedTeamAcls = oldAcls.Where(x => x.role == "assigned_in" && removedTeams.Contains(x.objectId)).ToList();
      _context.UserAcls.RemoveRange(removedTeamAcls);

      var newTeams = teams.Except(teamOldAcls);

      foreach (var teamId in newTeams)
      {
        commonRepository.addUserAcl(project.id, "project", "assigned_in", teamId, "team", companyId);
      }
      await _context.SaveChangesAsync();


      // ---------tag management---------

      var tagsOfCompany = await _context.Tags.Where(x => x.companyId == companyId).ToListAsync();
      var addedTagIds = tagsOfCompany.Where(x => tags.Contains(x.name)).Select(x => x.id).ToList();

      var oldTagAcls = await _context.TagAcls.Where(x => x.objectId == id && x.objectType == "project").ToListAsync();
      var oldTagIds = oldTagAcls.Select(x => x.tagId).ToList();
      var removedTagIds = oldTagIds.Except(addedTagIds);

      var removedOldTagAcls = oldTagAcls.Where(x => removedTagIds.Contains(x.tagId)).ToList();
      _context.TagAcls.RemoveRange(removedOldTagAcls);

      var newTagIds = addedTagIds.Except(oldTagIds);

      foreach (var newTagId in newTagIds)
      {
        commonRepository.addTagAcl(newTagId, project.id, "project");
      }

      foreach (var tagName in tags)
      {
        var existTag = tagsOfCompany.FirstOrDefault(x => x.name == tagName);
        if (existTag == null)
        {
          var newTag = commonRepository.addTag(tagName, companyId);
          await _context.SaveChangesAsync();
          var createdTagId = newTag.id;
          commonRepository.addTagAcl(createdTagId, project.id, "project");
        }
      }
      await _context.SaveChangesAsync();

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
      var tags = request.tags;
      
      //------project management ----
      var project = request.project;
      project.create_timestamp = DateTime.UtcNow;
      project.update_timestamp = DateTime.UtcNow;
      _context.Projects.Add(project);
      await _context.SaveChangesAsync();

      var createdProjectId = project.id;

      //------project usr acl management ----
      commonRepository.addUserAcl(createdProjectId, "project", "created_in", companyId, "company", companyId);

      foreach (var userId in projectManagers)
      {
        commonRepository.addUserAcl(userId, "user", "project_manager", createdProjectId, "project", companyId);
      }

      foreach (var userId in projectManagerAssistants)
      {
        commonRepository.addUserAcl(userId, "user", "project_assistant", createdProjectId, "project", companyId);
      }

      foreach (var teamId in teams)
      {
        commonRepository.addUserAcl(createdProjectId, "project", "assigned_in", teamId, "team", companyId);
      }
      await _context.SaveChangesAsync();

      //-------- tag management-----------
      var tagsOfCompany = await _context.Tags.Where(x => x.companyId == companyId).ToListAsync();

      foreach (var tagName in tags)
      {
        var existTag = tagsOfCompany.FirstOrDefault(x => x.name == tagName);
        if (existTag == null)
        {
          var newTag = commonRepository.addTag(tagName, companyId);
          await _context.SaveChangesAsync();
          var createdTagId = newTag.id;
          commonRepository.addTagAcl(createdTagId, createdProjectId, "project");
        }
        else
        {
          commonRepository.addTagAcl(existTag.id, createdProjectId, "project");
        }
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

      var oldTagAcls = await _context.TagAcls.Where(x => x.objectId == id && x.objectType == "project").ToListAsync();
      _context.TagAcls.RemoveRange(oldTagAcls);

      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool ProjectExists(long id)
    {
      return _context.Projects.Any(e => e.id == id);
    }
  }
}
