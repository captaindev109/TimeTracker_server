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
    public async Task<ActionResult<IEnumerable<Project>>> GetProjectsWithRoles(DataWithRolesRequest request)
    {
      var companyId = request.companyId;
      var userId = request.userId;
      List<string> roles = request.userRoles;

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
      return projects;
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
    public async Task<IActionResult> PutProject(long id, Project project)
    {
      if (id != project.id)
      {
        return BadRequest();
      }

      _context.Entry(project).State = EntityState.Modified;

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
    public async Task<ActionResult<Project>> PostProject(Project project)
    {
      _context.Projects.Add(project);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetProject", new { id = project.id }, project);
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
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool ProjectExists(long id)
    {
      return _context.Projects.Any(e => e.id == id);
    }
  }
}
