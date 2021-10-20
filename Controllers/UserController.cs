using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DataContracts.RequestBody;
using TimeTracker_server.Models;
using TimeTracker_server.Data;
using TimeTracker_server.Services;

namespace TimeTracker_server.Controllers
{
  // [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly MyDbContext _context;
    private readonly IConfiguration _config;
    public UserController(MyDbContext context, IConfiguration config)
    {
      _context = context;
      _config = config;
    }

    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
      return await _context.Users.ToListAsync();
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(long id)
    {
      var user = await _context.Users.FindAsync(id);

      if (user == null)
      {
        return NotFound();
      }

      return user;
    }

    // PUT: api/User/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(long id, User user)
    {
      if (id != user.id)
      {
        return BadRequest();
      }

      _context.Entry(user).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!UserExists(id))
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

    // POST: api/User
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetUser", new { id = user.id }, user);
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
      var user = await _context.Users.FindAsync(id);
      if (user == null)
      {
        return NotFound();
      }

      _context.Users.Remove(user);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool UserExists(long id)
    {
      return _context.Users.Any(e => e.id == id);
    }

    // POST: api/User/invite-user
    [HttpPost("invite-user")]
    public async Task<ActionResult<User>> InviteUser(RequestInviteUser request)
    {
      string email = request.email;
      string role = request.role;
      List<long> objectId = request.objectId;
      string objectType = request.objectType;
      string companyId = request.companyId.ToString();

      bool isExist = await _context.Users.AnyAsync(e => e.email == email);

      if (isExist == false)
      {
        var invitedUser = new User();
        invitedUser.email = email;
        invitedUser.status = "Invited";
        invitedUser.create_timestamp = DateTime.UtcNow;
        invitedUser.update_timestamp = DateTime.UtcNow;
        _context.Users.Add(invitedUser);
        await _context.SaveChangesAsync();
      }

      try
      {
        var token = TokenService.GenerateWebToken(
          new ClaimsIdentity(new Claim[]
          {
            new Claim("emailAddress", email),
            new Claim("roleText", role),
            new Claim("objectId", objectId.Count() > 0 ? string.Join(",", objectId) : ""),
            new Claim("objectType", objectType),
            new Claim("type", "invite_user"),
            new Claim("companyId", companyId),
          })
        );
        using StreamReader sr = new StreamReader("EmailTemplates/InviteUser.html");
        string body = sr.ReadToEnd().Replace("{full_name}", "There!").Replace("{token}", token);

        await EmailService.SendEmail(email, "Invitation Request", body);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return NoContent();
    }

    // POST: api/User/accept-invitation
    [HttpPost("accept-invitation")]
    public async Task<ActionResult<string>> AcceptInvitation(AcceptInvitationRequest request)
    {
      try
      {
        var senderEmail = request.senderEmail;
        var token = request.token;

        var tokenUser = TokenService.ReadWebToken(token);
        var email = tokenUser.FindFirst(claim => claim.Type == "emailAddress").Value;
        var role = tokenUser.FindFirst(claim => claim.Type == "roleText").Value;
        var objectIds = tokenUser.FindFirst(claim => claim.Type == "objectId").Value;
        var objectType = tokenUser.FindFirst(claim => claim.Type == "objectType").Value;
        var companyId = tokenUser.FindFirst(claim => claim.Type == "companyId").Value;

        var user = await _context.Users.FirstOrDefaultAsync(x => (x.email == email));

        if (user == null)
        {
          return NotFound("no user");
        }

        foreach (var objectId in objectIds.Split(',').ToList())
        {
          var newAcl = new UserAcl();
          newAcl.sourceId = user.id;
          newAcl.sourceType = "user";
          newAcl.role = role;
          newAcl.objectId = long.Parse(objectId);
          newAcl.objectType = objectType;

          var isExist = await _context.UserAcls.FirstOrDefaultAsync(x => x.sourceId == newAcl.sourceId && x.sourceType == "user" && x.role == newAcl.role && x.objectId == newAcl.objectId && x.objectType == newAcl.objectType);
          if (isExist != null)
          {
            break;
          }

          newAcl.create_timestamp = DateTime.UtcNow;
          newAcl.update_timestamp = DateTime.UtcNow;

          _context.UserAcls.Add(newAcl);
        }

        var roleAcl = new UserAcl();
        roleAcl.sourceId = user.id;
        roleAcl.sourceType = "user";
        roleAcl.role = role;
        roleAcl.objectId = long.Parse(companyId);
        roleAcl.objectType = "company";
        roleAcl.create_timestamp = DateTime.UtcNow;
        roleAcl.update_timestamp = DateTime.UtcNow;

        var isRoleExist = await _context.UserAcls.FirstOrDefaultAsync(x => x.sourceId == roleAcl.sourceId && x.sourceType == "user" && x.role == roleAcl.role && x.objectId == roleAcl.objectId && x.objectType == "company");
        if (isRoleExist != null)
        {
          _context.UserAcls.Add(roleAcl);
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          throw;
        }

      }
      catch (Exception ex)
      {
        throw ex;
      }
      return NoContent();
    }

    // GET: api/User/company/{companyId}
    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsersOfCompany(long companyId)
    {
      var userIds = new List<long>();

      var companyOwnerIds = await _context.UserAcls.Where(x => x.sourceType == "user" && (x.role == "company_admin" || x.role == "company_controller") && x.objectId == companyId).Select(x => x.sourceId).ToListAsync();
      var teamIds = await _context.UserAcls.Where(x => (x.sourceType == "team") && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
      var projectIds = await _context.UserAcls.Where(x => (x.sourceType == "project") && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
      var teamtUserIds = await _context.UserAcls.Where(x => x.sourceType == "user" && (x.role == "worker" || x.role == "team_lead") && x.objectType == "team" && teamIds.Contains(x.objectId)).Select(x => x.sourceId).ToListAsync();
      var projectUserIds = await _context.UserAcls.Where(x => x.sourceType == "user" && (x.role == "project_manager" || x.role == "project_assistant") && x.objectType == "project" && projectIds.Contains(x.objectId)).Select(x => x.sourceId).ToListAsync();
      userIds.AddRange(companyOwnerIds);
      userIds.AddRange(teamtUserIds);
      userIds.AddRange(projectUserIds);

      var resUsers = await _context.Users.Where(x => userIds.Contains(x.id)).ToListAsync();
      return resUsers;
    }

    // GET: api/User/with-roles/company/{companyId}
    [HttpGet("with-roles/company/{companyId}")]
    public async Task<ActionResult<IEnumerable<CompanyUserResponse>>> GetUsersWithRolesOfCompany(long companyId)
    {
      var userIds = new List<long>();

      var companyOwnerIds = await _context.UserAcls.Where(x => x.sourceType == "user" && (x.role == "company_admin" || x.role == "company_controller") && x.objectId == companyId).Select(x => x.sourceId).ToListAsync();
      var teamIds = await _context.UserAcls.Where(x => (x.sourceType == "team") && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
      var projectIds = await _context.UserAcls.Where(x => (x.sourceType == "project") && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
      var teamtUserIds = await _context.UserAcls.Where(x => x.sourceType == "user" && (x.role == "worker" || x.role == "team_lead") && x.objectType == "team" && teamIds.Contains(x.objectId)).Select(x => x.sourceId).ToListAsync();
      var projectUserIds = await _context.UserAcls.Where(x => x.sourceType == "user" && (x.role == "project_manager" || x.role == "project_assistant") && x.objectType == "project" && projectIds.Contains(x.objectId)).Select(x => x.sourceId).ToListAsync();
      userIds.AddRange(companyOwnerIds);
      userIds.AddRange(teamtUserIds);
      userIds.AddRange(projectUserIds);

      var companyUsers = new List<CompanyUserResponse>();

      var resUsers = await _context.Users.Where(x => userIds.Contains(x.id)).ToListAsync();
      foreach (User user in resUsers)
      {
        var filteredAcls = await _context.UserAcls.Where(x => x.sourceId == user.id && x.sourceType == "user").ToListAsync();

        var roleList = new List<string>();
        foreach (var acl in filteredAcls)
        {
          if (acl.objectType == "company" && acl.objectId == companyId)
          {
            roleList.Add(acl.role);
          }
          else
          {
            var relatedAcl = await _context.UserAcls.Where(x => x.sourceType == acl.objectType && x.sourceId == acl.objectId && x.objectType == "company" && x.objectId == companyId).FirstOrDefaultAsync();
            if (relatedAcl != null)
            {
              roleList.Add(acl.role);
            }
          }
        }

        var roles = roleList.Distinct().ToList();

        var companyUser = new CompanyUserResponse();
        companyUser.user = user;
        companyUser.roles = roles;

        companyUsers.Add(companyUser);
      }

      return companyUsers;
    }

    [HttpGet("{userId}/company/{companyId}/roles")]
    public async Task<ActionResult<CompanyUserRoleResponse>> GetUserRolesOfCompany(long userId, long companyId)
    {
      var userRoles = new CompanyUserRoleResponse();
      var user = await _context.Users.FindAsync(userId);

      userRoles.user = user;

      var isCompanyAdmin = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "company_admin" && x.objectId == companyId && x.objectType == "company").FirstOrDefaultAsync();
      userRoles.is_company_admin = isCompanyAdmin != null ? true : false;

      var isCompanyController = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "company_controller" && x.objectId == companyId && x.objectType == "company").FirstOrDefaultAsync();
      userRoles.is_company_controller = isCompanyController != null ? true : false;

      var teamIds = await _context.UserAcls.Where(x => x.sourceType == "team" && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
      var projectIds = await _context.UserAcls.Where(x => x.sourceType == "project" && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();

      var worker = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "worker" && x.objectType == "team" && teamIds.Contains(x.objectId)).Select(x => x.objectId).ToListAsync();
      var team_lead = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "team_lead" && x.objectType == "team" && teamIds.Contains(x.objectId)).Select(x => x.objectId).ToListAsync();
      var project_manager = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "project_manager" && x.objectType == "project" && projectIds.Contains(x.objectId)).Select(x => x.objectId).ToListAsync();
      var project_assistant = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "project_assistant" && x.objectType == "project" && projectIds.Contains(x.objectId)).Select(x => x.objectId).ToListAsync();

      userRoles.worker_teams = worker;
      userRoles.team_lead_teams = team_lead;
      userRoles.project_manager_projects = project_manager;
      userRoles.project_assistant_projects = project_assistant;

      return userRoles;
    }

    [HttpPost("update-roles")]
    public async Task<ActionResult<string>> UpdateUserRoles(UpdateUserRolesRequest request)
    {
      // var userId = request.userId;
      // var companyId = request.companyId;
      // var is_company_admin = request.is_company_admin;
      // var is_company_controller = request.is_company_controller;
      // var project_manager_projects = request.project_manager_projects;
      // var project_assistant_projects = request.project_assistant_projects;
      // var team_lead_teams = request.team_lead_teams;
      // var worker_teams = request.worker_teams;

      // var isCompanyAdmin = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "company_admin" && x.objectId == companyId && x.objectType == "company").FirstOrDefaultAsync();


      // var isCompanyController = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "company_controller" && x.objectId == companyId && x.objectType == "company").FirstOrDefaultAsync();


      // var teamIds = await _context.UserAcls.Where(x => x.sourceType == "team" && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
      // var projectIds = await _context.UserAcls.Where(x => x.sourceType == "project" && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();

      // var worker = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "worker" && x.objectType == "team" && teamIds.Contains(x.objectId)).Select(x => x.objectId).ToListAsync();
      // var team_lead = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "team_lead" && x.objectType == "team" && teamIds.Contains(x.objectId)).Select(x => x.objectId).ToListAsync();
      // var project_manager = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "project_manager" && x.objectType == "project" && projectIds.Contains(x.objectId)).Select(x => x.objectId).ToListAsync();
      // var project_assistant = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && x.role == "project_assistant" && x.objectType == "project" && projectIds.Contains(x.objectId)).Select(x => x.objectId).ToListAsync();

      return NoContent();
    }
  }
}
