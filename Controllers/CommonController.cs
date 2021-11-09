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
  [Route("[controller]")]
  [ApiController]
  public class CommonController : ControllerBase
  {
    private readonly MyDbContext _context;

    public CommonController(MyDbContext context)
    {
      _context = context;
    }
    public async Task<List<string>> getRoles(long companyId, long userId)
    {
     var roleList = await _context.UserAcls.Where(x => x.sourceId == userId && x.sourceType == "user" && !x.role.Contains("member") && x.objectType == "company" && x.objectId == companyId).Select(x => x.role).Distinct().ToListAsync();

      return roleList;
    }
   
  }
}
