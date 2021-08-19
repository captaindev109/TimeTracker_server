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
using CompanyApi.Models;
using DataContracts.RequestBody;
using UserApi.Models;

namespace TimeTracker_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyContext _context;

        public CompanyController(CompanyContext context)
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

        // POST: api/Company
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompany", new { id = company.id }, company);
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

        // POST: api/create_with_signup
        [HttpPost("create_with_signup")]
        public async Task<ActionResult<Company>> CreateCompanyWithSignup(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompany", new { id = company.id }, company);
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
