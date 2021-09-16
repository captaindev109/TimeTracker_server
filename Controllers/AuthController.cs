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

        // POST: api/auth/select-company
        [HttpPost("select-company")]
        public async Task<ActionResult<IEnumerable<Company>>> CreateCompanyWithSignup(Company company)
        {
//SELECT DISTINCT "objectId" FROM public."UserAcls" WHERE "objectType"='company' AND (
//CONCAT("sourceId", "sourceType")=ANY(SELECT DISTINCT CONCAT("objectId", "objectType") 
// FROM public."UserAcls" WHERE "sourceId" = 1 AND "sourceType" = 'user') OR ("sourceId"=1 AND "sourceType"='user'))

            // return await _context.Companies.ToListAsync();
            // var filteredCompanies = _context.UserAcls.Where(x => x.objectType == "company").Select(x => x.objectId ).Distinct();
            // _context.Companies.Add(company);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction("GetCompany", new { id = company.id }, company);
            return NoContent();
        }

    }
}
