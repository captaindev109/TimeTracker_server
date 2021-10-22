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

namespace TimeTracker_server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CompanyController : ControllerBase
  {
    private readonly MyDbContext _context;

    public CompanyController(MyDbContext context)
    {
      _context = context;
    }

    // GET: api/Company
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
    {
      return await _context.Companies.ToListAsync();
    }

    // GET: api/Company/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Company>> GetCompany(long id)
    {
      var company = await _context.Companies.FindAsync(id);

      if (company == null)
      {
        return NotFound();
      }

      return company;
    }

    // PUT: api/Company/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCompany(long id, Company company)
    {
      if (id != company.id)
      {
        return BadRequest();
      }

      _context.Entry(company).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!CompanyExists(id))
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

    // DELETE: api/Company/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(long id)
    {
      var company = await _context.Companies.FindAsync(id);
      if (company == null)
      {
        return NotFound();
      }

      _context.Companies.Remove(company);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool CompanyExists(long id)
    {
      return _context.Companies.Any(e => e.id == id);
    }

    // POST: api/company/create
    [HttpPost("create")]
    public async Task<ActionResult<Company>> CreateCompany(CreateCompanyRequest request)
    {
      var companyName = request.companyName;
      var userId = request.userId;

      var newCompany = new Company();
      newCompany.name = companyName;
      newCompany.status = "Active";
      newCompany.create_timestamp = DateTime.UtcNow;
      newCompany.update_timestamp = DateTime.UtcNow;

      _context.Companies.Add(newCompany);
      await _context.SaveChangesAsync();
      var createdCompanyId = newCompany.id;

      var userAcl = new UserAcl();
      userAcl.sourceId = userId;
      userAcl.sourceType = "user";
      userAcl.role = "company_owner";
      userAcl.objectId = createdCompanyId;
      userAcl.objectType = "company";
      userAcl.create_timestamp = DateTime.UtcNow;
      userAcl.update_timestamp = DateTime.UtcNow;
      _context.UserAcls.Add(userAcl);

      var adminAcl = new UserAcl();
      adminAcl.sourceId = userId;
      adminAcl.sourceType = "user";
      adminAcl.role = "company_admin";
      adminAcl.objectId = createdCompanyId;
      adminAcl.objectType = "company";
      adminAcl.create_timestamp = DateTime.UtcNow;
      adminAcl.update_timestamp = DateTime.UtcNow;
      _context.UserAcls.Add(adminAcl);

      var memberAcl = new UserAcl();
      memberAcl.sourceId = userId;
      memberAcl.sourceType = "user";
      memberAcl.role = "active_member";
      memberAcl.objectId = createdCompanyId;
      memberAcl.objectType = "company";
      memberAcl.create_timestamp = DateTime.UtcNow;
      memberAcl.update_timestamp = DateTime.UtcNow;
      _context.UserAcls.Add(memberAcl);
      

      var user = _context.Users.Find(userId);
      if (user == null)
      {
        return NoContent();
      }
      else
      {
        user.status = "Active";
        _context.Entry(user).State = EntityState.Modified;
      }
      await _context.SaveChangesAsync();
      return newCompany;
    }

    // POST: api/join_company_signup
    [HttpPost("join_company_signup")]
    public async Task<ActionResult<Company>> JoinCompanyWithSignup(RequestJoinUser userBody)
    {
      var ownerEmail = userBody.ownerEmail;
      var userName = userBody.userName;

      try
      {
        await SendCompanyJoinRequest(userName, "will", ownerEmail);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return null;
    }

    private async Task SendCompanyJoinRequest(string name, string token, string to)
    {
      try
      {
        using StreamReader sr = new StreamReader("EmailTemplates/JoinRequest.html");
        string s = sr.ReadToEnd();
        string body = s.Replace("{request_user_name}", name)
            .Replace("{token}", token);

        SmtpClient client = new SmtpClient("robot@t22.tools");
        client.UseDefaultCredentials = false;
        client.EnableSsl = true;
        client.Port = 587;
        client.Host = "host212.checkdomain.de";
        client.Credentials = new NetworkCredential("robot@t22.tools", "?T6D2e#r0%p?mA4G");

        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress("robot@t22.tools", "Request Join to company");
        mailMessage.To.Add(to);
        mailMessage.Body = body;
        mailMessage.IsBodyHtml = true;
        mailMessage.Subject = "Request Join to company";
        await client.SendMailAsync(mailMessage);

      }
      catch (Exception ex)
      {
        throw ex;
      }
    }
  }
}
