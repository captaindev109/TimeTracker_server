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

    public UserAcl addUserAcl(long sourceId, string sourceType, string role, long objectId, string objectType, long companyId)
    {
      var newAcl = new UserAcl();
      newAcl.sourceId = sourceId;
      newAcl.sourceType = sourceType;
      newAcl.role = role;
      newAcl.objectId = objectId;
      newAcl.objectType = objectType;
      newAcl.companyId = companyId;
      newAcl.create_timestamp = DateTime.UtcNow;
      newAcl.update_timestamp = DateTime.UtcNow;

      _context.UserAcls.Add(newAcl);

      return newAcl;
    }

    public UserAcl removeUserAcl(UserAcl acl)
    {
      _context.UserAcls.Remove(acl);
      return acl;
    }
    public TagAcl addTagAcl(long tagId, long objectId, string objectType)
    {
      var tagAcl = new TagAcl();
      tagAcl.tagId = tagId;
      tagAcl.objectType = objectType;
      tagAcl.objectId = objectId;
      tagAcl.create_timestamp = DateTime.UtcNow;
      tagAcl.update_timestamp = DateTime.UtcNow;

      _context.TagAcls.Add(tagAcl);

      return tagAcl;
    }

    public Tag addTag(string name, long companyId)
    {
      var newTag = new Tag();
      newTag.name = name;
      newTag.companyId = companyId;
      newTag.create_timestamp = DateTime.UtcNow;
      newTag.update_timestamp = DateTime.UtcNow;
      _context.Tags.Add(newTag);

      return newTag;
    }
  }
}
