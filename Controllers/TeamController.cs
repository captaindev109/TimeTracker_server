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
  public class TeamController : ControllerBase
  {
    private readonly MyDbContext _context;

    public TeamController(MyDbContext context)
    {
      _context = context;
    }

    // GET: api/Team
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
    {
      return await _context.Teams.ToListAsync();
    }
    // POST: api/Team/with-roles
    [HttpPost("with-roles")]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjectsWithRoles(DataWithRolesRequest request)
    {
      var companyId = request.companyId;
      var userId = request.userId;
      List<string> roles = request.userRoles;

      var teamIds = new List<long>();
      if (roles.Contains("company_admin") || roles.Contains("company_controller"))
      {
        var tmpTeamIds = await _context.UserAcls.Where(x => x.sourceType == "team" && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
        teamIds.AddRange(tmpTeamIds);
      }
      else if (roles.Contains("team_lead") || roles.Contains("worker"))
      {
        var tmpTeamIds = await _context.UserAcls.Where(x => (x.role == "team_lead" || x.role == "worker") && x.sourceType == "user" && x.sourceId == userId && x.objectType == "team").Select(x => x.objectId).ToListAsync();
        teamIds.AddRange(tmpTeamIds);
      }
      else if (roles.Contains("project_manager") || roles.Contains("project_assistant"))
      {
        var projectIds = await _context.UserAcls.Where(x => (x.role == "project_manager" || x.role == "project_assistant") && x.sourceType == "user" && x.sourceId == userId && x.objectType == "project").Select(x => x.objectId).ToListAsync();
        var tmpTeamIds = await _context.UserAcls.Where(x => x.sourceType == "project" && x.role == "assigned_in" && x.objectType == "team" && teamIds.Contains(x.sourceId)).Select(x => x.objectId).ToListAsync();
        teamIds.AddRange(tmpTeamIds);
      }

      return await _context.Projects.Where(x => teamIds.Contains(x.id)).ToListAsync();
    }
    // GET: api/Team/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Team>> GetTeam(long id)
    {
      var team = await _context.Teams.FindAsync(id);

      if (team == null)
      {
        return NotFound();
      }

      return team;
    }

    // PUT: api/Team/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTeam(long id, Team team)
    {
      if (id != team.id)
      {
        return BadRequest();
      }

      _context.Entry(team).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!TeamExists(id))
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

    // POST: api/Team
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Team>> PostTeam(Team team)
    {
      _context.Teams.Add(team);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetTeam", new { id = team.id }, team);
    }

    // DELETE: api/Team/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(long id)
    {
      var team = await _context.Teams.FindAsync(id);
      if (team == null)
      {
        return NotFound();
      }

      _context.Teams.Remove(team);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool TeamExists(long id)
    {
      return _context.Teams.Any(e => e.id == id);
    }
  }
}
