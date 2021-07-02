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
using UserApi.Models;

namespace TimeTracker_server.Controllers
{
  // [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly UserContext _context;
    private readonly IConfiguration _config;
    public UserController(UserContext context, IConfiguration config)
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

    // POST: api/User/login
    [HttpPost("login")]
    public async Task<ActionResult<User>> LoginUser(User userBody)
    {
      var email = userBody.email;
      var user = await _context.Users.FirstOrDefaultAsync(x => (x.email == email && x.password == userBody.password));
      if (user == null)
      {
        return NotFound();
      }

      return user;
    }

    // POST: api/User/forgot-password
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
        var code = GenerateJsonWebToken(user.email, "user.role", user.id, "forgot_password");
        await SendForgotPassword(user.firstName + user.lastName, code, email);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return user;
    }
    private bool UserExists(long id)
    {
      return _context.Users.Any(e => e.id == id);
    }

    private string GenerateJsonWebToken(string email, string role, long userId, string tokenType)
    {
      var claims = new List<Claim>
            {
                new Claim("email", email),
                new Claim("type", tokenType),
                // new Claim(ClaimTypes.Role,role),
                new Claim("id", userId.ToString()),
            };


      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Secret));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

      var token = new JwtSecurityToken(_config["Jwt:Issuer"],
        _config["Jwt:Issuer"],
        claims,
        expires: DateTime.Now.AddYears(1),
        signingCredentials: credentials);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task SendForgotPassword(string name, string token, string to)
    {
      try
      {
        using StreamReader sr = new StreamReader("EmailTemplates/ForgotPassword.html");
        string s = sr.ReadToEnd();
        string body = s.Replace("{full_name}", name)
            .Replace("{token}", token);

        SmtpClient client = new SmtpClient("robot@t22.tools");
        client.UseDefaultCredentials = false;
        client.EnableSsl = true;
        client.Port = 587;
        client.Host = "host212.checkdomain.de";
        client.Credentials = new NetworkCredential("robot@t22.tools", "?T6D2e#r0%p?mA4G");

        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress("robot@t22.tools", "Reset password");
        mailMessage.To.Add(to);
        mailMessage.Body = body;
        mailMessage.IsBodyHtml = true;
        mailMessage.Subject = "Reset password";
        await client.SendMailAsync(mailMessage);

      }
      catch (Exception ex)
      {
        throw ex;
      }
    }
  }
}
