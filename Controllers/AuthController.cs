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
using DataContracts.RequestBody;
using TimeTracker_server.Models;
using TimeTracker_server.Data;
using TimeTracker_server.Services;

namespace TimeTracker_server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly MyDbContext _context;

    public AuthController(MyDbContext context)
    {
      _context = context;
    }

    // GET: api/auth/get-company/5
    [HttpGet("get-company/{userId}")]
    public async Task<ActionResult<IEnumerable<Company>>> SelectCompany(long userId)
    {
      var filteredAcls = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user").ToListAsync();

      var companyAcls = new List<long>();
      foreach (var acl in filteredAcls)
      {
        if (acl.objectType == "company")
        {
          companyAcls.Add(acl.objectId);
        }
        else
        {
          var relatedAcl = await _context.UserAcls.Where(x => x.sourceType == acl.objectType && x.sourceId == acl.objectId && x.objectType == "company").FirstOrDefaultAsync();
          companyAcls.Add(relatedAcl.objectId);
        }
      }

      var filteredCompanies = await _context.Companies.Where(x => companyAcls.Contains(x.id)).ToListAsync();

      if (filteredCompanies == null)
      {
        return NoContent();
      }
      return filteredCompanies;
    }

    // GET: api/auth/get-roles/use/5/company/4
    [HttpGet("get-roles/user/{userId}/company/{companyId}")]
    public async Task<ActionResult<List<string>>> getRoles(long userId, long companyId)
    {

      var filteredAcls = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user").ToListAsync();

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

      return roleList.Distinct().ToList();
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<User>> LoginUser(LoginRequest request)
    {
      var email = request.email;
      var password = request.password;

      var user = await _context.Users.FirstOrDefaultAsync(x => x.email == email);

      if (user == null)
      {
        return NotFound();
      }
      else
      {
        if (user.status == "Invited")
        {
          return StatusCode(405);
        }
        if (user.password == password)
        {
          if (user.status == "Locked")
          {
            return StatusCode(403);
          }
        }
        else
        {
          return NotFound();
        }
      }


      return user;
    }

    // POST: api/auth/signup
    [HttpPost("signup")]
    public async Task<ActionResult<User>> SignupUser(SignupRequest request)
    {
      var email = request.email;

      bool isExist = await _context.Users.AnyAsync(e => e.email == email);


      if (isExist == true)
      {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.email == email);
        if (user.status == "Invited")
        {
          user.firstName = request.firstName;
          user.lastName = request.lastName;
          user.password = request.password;
          user.update_timestamp = DateTime.UtcNow;
          user.status = "Active";

          _context.Entry(user).State = EntityState.Modified;
          await _context.SaveChangesAsync();

          return user;
        }
        else
        {
          return StatusCode(404);
        }
      }
      else
      {
        var user = new User();
        user.email = email;
        user.firstName = request.firstName;
        user.lastName = request.lastName;
        user.password = request.password;
        user.create_timestamp = DateTime.UtcNow;
        user.update_timestamp = DateTime.UtcNow;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
      }
    }

    // PUT: api/auth/update-profile
    [HttpPut("update-profile")]
    public async Task<ActionResult<User>> UpdateProfile(User user)
    {
      _context.Entry(user).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!UserExists(user.id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return user;
    }

    // POST: api/auth/forgot-password
    [HttpPost("forgot-password")]
    public async Task<ActionResult<User>> RequestForgotPassword(User userBody)
    {
      var email = userBody.email;
      var user = await _context.Users.FirstOrDefaultAsync(x => (x.email == email));
      if (user == null)
      {
        return BadRequest(new { message = "There are no accounts linked to this email!" });
      }

      try
      {
        var token = TokenService.GenerateWebToken(
          new ClaimsIdentity(new Claim[]
          {
            new Claim("emailAdress", user.email),
            new Claim("type", "forgot_password"),
            new Claim("id", user.id.ToString()),
          })
        );

        using StreamReader sr = new StreamReader("EmailTemplates/ForgotPassword.html");
        string body = sr.ReadToEnd().Replace("{full_name}", user.firstName + user.lastName).Replace("{token}", token);
        await EmailService.SendEmail(email, "Forgot Password Request", body);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return user;
    }

    // POST: api/auth/reset-password
    [HttpPost("reset-password")]
    public async Task<ActionResult<User>> ResetPassword(ResetPasswordRequest request)
    {
      try
      {
        var password = request.password;
        var token = request.token;
        var tokenUser = TokenService.ReadWebToken(token);
        var id = tokenUser.FindFirst(claim => claim.Type == "id").Value;
        var user = await _context.Users.FirstOrDefaultAsync(x => (x.id.ToString() == id));

        if (user == null)
        {
          return NotFound();
        }

        user.password = password;
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
    private bool UserExists(long id)
    {
      return _context.Users.Any(e => e.id == id);
    }
  }
}
