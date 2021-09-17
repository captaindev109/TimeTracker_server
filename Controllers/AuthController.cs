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
    [HttpGet("get-company/{id}")]
    public async Task<ActionResult<IEnumerable<Company>>> SelectCompany(long id)
    {
      //SELECT DISTINCT "objectId" FROM public."UserAcls" WHERE "objectType"='company' AND (
      //CONCAT("sourceId", "sourceType")=ANY(SELECT DISTINCT CONCAT("objectId", "objectType") 
      // FROM public."UserAcls" WHERE "sourceId" = 1 AND "sourceType" = 'user') OR ("sourceId"=1 AND "sourceType"='user'))

      var filteredAcls = await _context.UserAcls.Where(x => x.sourceId == id && x.sourceType == "user").ToListAsync();

      var companyAcls = new List<long>();
      foreach (var acl in filteredAcls)
      {
        if (acl.objectType == "company")
        {
          companyAcls.Add(acl.objectId);
        }
        else
        {
          var relatedAcl = await _context.UserAcls.Where(x => x.sourceType == acl.objectType && x.sourceId == acl.objectId && x.objectType == "company").FirstAsync();
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

  }
}
