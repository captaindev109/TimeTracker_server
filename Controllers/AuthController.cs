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
  }
}
