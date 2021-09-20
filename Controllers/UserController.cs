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
            new Claim("objectId", string.Join(",", objectId)),
            new Claim("objectType", objectType),
            new Claim("type", "invite_user"),
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
  }
}
