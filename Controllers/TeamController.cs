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
    public async Task<ActionResult<IEnumerable<TeamWithRoleResponse>>> GetProjectsWithRoles(DataWithRolesRequest request)
    {
      var companyId = request.companyId;
      var userId = request.userId;
      List<string> roles = getRoles(companyId, userId).Result;

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
      var editableTeamIds = await _context.UserAcls.Where(x => x.role == "team_lead" && x.sourceType == "user" && x.sourceId == userId && x.objectType == "team").Select(x => x.objectId).ToListAsync();

      var teams = await _context.Teams.Where(x => teamIds.Contains(x.id)).ToListAsync();
      var resTeams = new List<TeamWithRoleResponse>();

      foreach (var item in teams)
      {
        var isEditable = false;
        if (editableTeamIds.Contains(item.id))
        {
          isEditable = true;
        }
        var teamItem = new TeamWithRoleResponse();
        teamItem.team = item;
        teamItem.isEditable = isEditable;

        var teamLeadAcls = await _context.UserAcls.Where(x => x.sourceType == "user" && x.role == "team_lead" && x.objectId == item.id && x.objectType == "team").Select(x => x.sourceId).ToListAsync();
        teamItem.teamLeads = await _context.Users.Where(x => teamLeadAcls.Contains(x.id)).ToListAsync();

        var memberAcls = await _context.UserAcls.Where(x => x.sourceType == "user" && x.role == "worker" && x.objectId == item.id && x.objectType == "team").Select(x => x.sourceId).ToListAsync();
        teamItem.members = await _context.Users.Where(x => memberAcls.Contains(x.id)).ToListAsync();

        resTeams.Add(teamItem);
      }

      return resTeams;
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
    public async Task<IActionResult> PutTeam(long id, UpdateTeamRequest request)
    {
      var team = request.team;
      var teamLeads = request.teamLeads;
      var members = request.members;
      var companyId = request.companyId;

      if (id != team.id)
      {
        return BadRequest();
      }

      _context.Entry(team).State = EntityState.Modified;

      var oldAcls = await _context.UserAcls.Where(x => x.sourceType == "user" && (x.role == "team_lead" || x.role == "worker") && x.objectId == team.id && x.objectType == "team").ToListAsync();

      _context.UserAcls.RemoveRange(oldAcls);

      foreach (var userId in teamLeads)
      {
        var newAcl = new UserAcl();
        newAcl.sourceId = userId;
        newAcl.sourceType = "user";
        newAcl.role = "team_lead";
        newAcl.objectId = team.id;
        newAcl.objectType = "team";
        newAcl.companyId = companyId;
        newAcl.create_timestamp = DateTime.UtcNow;
        newAcl.update_timestamp = DateTime.UtcNow;

        _context.UserAcls.Add(newAcl);
      }

      foreach (var userId in members)
      {
        var newAcl = new UserAcl();
        newAcl.sourceId = userId;
        newAcl.sourceType = "user";
        newAcl.role = "worker";
        newAcl.objectId = team.id;
        newAcl.objectType = "team";
        newAcl.companyId = companyId;
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
    public async Task<ActionResult<Team>> PostTeam(CreateTeamRequest request)
    {
      var companyId = request.companyId;
      var teamLeads = request.teamLeads;
      var members = request.members;

      var team = request.team;
      team.create_timestamp = DateTime.UtcNow;
      team.update_timestamp = DateTime.UtcNow;
      _context.Teams.Add(team);
      await _context.SaveChangesAsync();

      var createdTeamId = team.id;

      var userAcl = new UserAcl();
      userAcl.sourceId = createdTeamId;
      userAcl.sourceType = "team";
      userAcl.role = "created_in";
      userAcl.objectId = companyId;
      userAcl.objectType = "company";
      userAcl.companyId = companyId;
      userAcl.create_timestamp = DateTime.UtcNow;
      userAcl.update_timestamp = DateTime.UtcNow;
      _context.UserAcls.Add(userAcl);

      foreach (var userId in teamLeads)
      {
        var newAcl = new UserAcl();
        newAcl.sourceId = userId;
        newAcl.sourceType = "user";
        newAcl.role = "team_lead";
        newAcl.objectId = createdTeamId;
        newAcl.objectType = "team";
        newAcl.companyId = companyId;
        newAcl.create_timestamp = DateTime.UtcNow;
        newAcl.update_timestamp = DateTime.UtcNow;

        _context.UserAcls.Add(newAcl);
      }

      foreach (var userId in members)
      {
        var newAcl = new UserAcl();
        newAcl.sourceId = userId;
        newAcl.sourceType = "user";
        newAcl.role = "worker";
        newAcl.objectId = createdTeamId;
        newAcl.objectType = "team";
        newAcl.companyId = companyId;
        newAcl.create_timestamp = DateTime.UtcNow;
        newAcl.update_timestamp = DateTime.UtcNow;

        _context.UserAcls.Add(newAcl);
      }

      await _context.SaveChangesAsync();
      return team;
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

      var oldAcls = await _context.UserAcls.Where(x =>
       (x.objectType == "company" && x.role == "created_in" && x.sourceId == id && x.sourceType == "team")
    || (x.sourceType == "user" && (x.role == "team_lead" || x.role == "worker") && x.objectId == id && x.objectType == "team")
    || (x.sourceType == "project" && x.role == "assigned_in" && x.sourceId == id && x.objectType == "team")
     ).ToListAsync();

      _context.UserAcls.RemoveRange(oldAcls);

      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool TeamExists(long id)
    {
      return _context.Teams.Any(e => e.id == id);
    }

    private async Task<List<string>> getRoles(long companyId, long userId)
    {
      var roleList = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && !x.role.Contains("member") && x.objectType == "company" && x.objectId == companyId).Select(x => x.role).Distinct().ToListAsync();

      return roleList;
    }
  }
}
