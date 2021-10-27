using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker_server.Models;
using TimeTracker_server.Data;
using DataContracts.RequestBody;

namespace TimeTracker_server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ResourceTypeController : ControllerBase
  {
    private readonly MyDbContext _context;

    public ResourceTypeController(MyDbContext context)
    {
      _context = context;
    }

    // GET: api/ResourceType
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResourceType>>> GetResourceTypes()
    {
      return await _context.ResourceTypes.ToListAsync();
    }

    // GET: api/ResourceType/company/5
    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<IEnumerable<ResourceTypeResponse>>> GetResourceTypesOfCompany(long companyId)
    {
      var resourceTypeIds = await _context.UserAcls.Where(x => x.sourceType == "resourceType" && x.role == "created_in" && x.objectId == companyId && x.objectType == "company").Select(x => x.sourceId).ToListAsync();
      Console.WriteLine(resourceTypeIds);
      var resourceTypes = await _context.ResourceTypes.Where(x => resourceTypeIds.Contains(x.id)).ToListAsync();

      var resResourceTypes = new List<ResourceTypeResponse>();
      foreach (var item in resourceTypes)
      {
        var tagIds = await _context.TagAcls.Where(x => x.objectId == item.id && x.objectType == "resourceType").Select(x => x.tagId).ToListAsync();
        var tags = await _context.Tags.Where(x => tagIds.Contains(x.id)).ToListAsync();
        var resourceTypeItem = new ResourceTypeResponse();
        resourceTypeItem.resourceType = item;
        resourceTypeItem.tags = tags;
        resResourceTypes.Add(resourceTypeItem);
      }

      return resResourceTypes;
    }

    // GET: api/ResourceType/tags/company/5
    [HttpGet("tags/company/{companyId}")]
    public async Task<ActionResult<IEnumerable<Tag>>> GetResourceTypeTagsOfCompany(long companyId)
    {

      var tagIds = await _context.TagAcls.Where(x => x.objectType == "company" && x.objectId == companyId).Select(x => x.tagId).ToListAsync();
      var tags = await _context.Tags.Where(x => tagIds.Contains(x.id) && x.type == "resourceType").ToListAsync();

      return tags;
    }

    // GET: api/ResourceType/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ResourceType>> GetResourceType(long id)
    {
      var resourceType = await _context.ResourceTypes.FindAsync(id);

      if (resourceType == null)
      {
        return NotFound();
      }

      return resourceType;
    }

    // PUT: api/ResourceType/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutResourceType(long id, UpdateResourceTypeRequestRequest request)
    {
      var resourceType = request.resourceType;
      var companyId = request.companyId;
      var tags = request.tags;

      if (id != resourceType.id)
      {
        return BadRequest();
      }

      _context.Entry(resourceType).State = EntityState.Modified;

      var tagsAcl = await _context.TagAcls.Where(x => x.objectId == id && x.objectType == "resourceType").ToListAsync();
      _context.TagAcls.RemoveRange(tagsAcl);
      await _context.SaveChangesAsync();

      var tagIds = await _context.TagAcls.Where(x => x.objectType == "company" && x.objectId == companyId).Select(x => x.tagId).ToListAsync();
      var tagsOfCompany = await _context.Tags.Where(x => tagIds.Contains(x.id)).ToListAsync();

      foreach (var tagName in tags)
      {
        var existTag = tagsOfCompany.FirstOrDefault(x => x.name == tagName);
        if (existTag == null)
        {
          var newTag = new Tag();
          newTag.name = tagName;
          newTag.type = "resourceType";
          newTag.create_timestamp = DateTime.UtcNow;
          newTag.update_timestamp = DateTime.UtcNow;
          _context.Tags.Add(newTag);
          await _context.SaveChangesAsync();

          var createdTagId = newTag.id;

          var tagAcl = new TagAcl();
          tagAcl.tagId = createdTagId;
          tagAcl.objectType = "resourceType";
          tagAcl.objectId = resourceType.id;
          tagAcl.create_timestamp = DateTime.UtcNow;
          tagAcl.update_timestamp = DateTime.UtcNow;
          _context.TagAcls.Add(tagAcl);

          var tagAclCompany = new TagAcl();
          tagAclCompany.tagId = createdTagId;
          tagAclCompany.objectType = "company";
          tagAclCompany.objectId = companyId;
          tagAclCompany.create_timestamp = DateTime.UtcNow;
          tagAclCompany.update_timestamp = DateTime.UtcNow;
          _context.TagAcls.Add(tagAclCompany);
        }
        else
        {
          var tagAcl = new TagAcl();
          tagAcl.tagId = existTag.id;
          tagAcl.objectType = "resourceType";
          tagAcl.objectId = resourceType.id;
          tagAcl.create_timestamp = DateTime.UtcNow;
          tagAcl.update_timestamp = DateTime.UtcNow;
          _context.TagAcls.Add(tagAcl);
        }
      }

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ResourceTypeExists(id))
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

    // POST: api/ResourceType
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ResourceType>> PostResourceType(CreateResourceTypeRequestRequest request)
    {
      var resourceType = request.resourceType;
      resourceType.create_timestamp = DateTime.UtcNow;
      resourceType.update_timestamp = DateTime.UtcNow;
      var companyId = request.companyId;
      var tags = request.tags;

      _context.ResourceTypes.Add(resourceType);
      await _context.SaveChangesAsync();

      var createdResourceTypeId = resourceType.id;

      var resourceTypeAcl = new UserAcl();
      resourceTypeAcl.sourceId = createdResourceTypeId;
      resourceTypeAcl.sourceType = "resourceType";
      resourceTypeAcl.role = "created_in";
      resourceTypeAcl.objectId = companyId;
      resourceTypeAcl.objectType = "company";
      resourceTypeAcl.create_timestamp = DateTime.UtcNow;
      resourceTypeAcl.update_timestamp = DateTime.UtcNow;
      _context.UserAcls.Add(resourceTypeAcl);
      await _context.SaveChangesAsync();

      var tagIds = await _context.TagAcls.Where(x => x.objectType == "company" && x.objectId == companyId).Select(x => x.tagId).ToListAsync();
      var tagsOfCompany = await _context.Tags.Where(x => tagIds.Contains(x.id)).ToListAsync();

      foreach (var tagName in tags)
      {
        var existTag = tagsOfCompany.FirstOrDefault(x => x.name == tagName);
        if (existTag == null)
        {
          var newTag = new Tag();
          newTag.name = tagName;
          newTag.type = "resourceType";
          newTag.create_timestamp = DateTime.UtcNow;
          newTag.update_timestamp = DateTime.UtcNow;
          _context.Tags.Add(newTag);
          await _context.SaveChangesAsync();

          var createdTagId = newTag.id;

          var tagAcl = new TagAcl();
          tagAcl.tagId = createdTagId;
          tagAcl.objectType = "resourceType";
          tagAcl.objectId = createdResourceTypeId;
          tagAcl.create_timestamp = DateTime.UtcNow;
          tagAcl.update_timestamp = DateTime.UtcNow;
          _context.TagAcls.Add(tagAcl);

          var tagAclCompany = new TagAcl();
          tagAclCompany.tagId = createdTagId;
          tagAclCompany.objectType = "company";
          tagAclCompany.objectId = companyId;
          tagAclCompany.create_timestamp = DateTime.UtcNow;
          tagAclCompany.update_timestamp = DateTime.UtcNow;
          _context.TagAcls.Add(tagAclCompany);
        }
        else
        {
          var tagAcl = new TagAcl();
          tagAcl.tagId = existTag.id;
          tagAcl.objectType = "resourceType";
          tagAcl.objectId = createdResourceTypeId;
          tagAcl.create_timestamp = DateTime.UtcNow;
          tagAcl.update_timestamp = DateTime.UtcNow;
          _context.TagAcls.Add(tagAcl);
        }
      }
      await _context.SaveChangesAsync();

      return resourceType;
    }

    // DELETE: api/ResourceType/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResourceType(long id)
    {
      var resourceType = await _context.ResourceTypes.FindAsync(id);
      if (resourceType == null)
      {
        return NotFound();
      }

      var resourceTypeAcl = await _context.UserAcls.Where(x => x.sourceId == id && x.sourceType == "resourceType" && x.role == "created_in").FirstOrDefaultAsync();
      _context.UserAcls.Remove(resourceTypeAcl);

      var tagsAcl = await _context.TagAcls.Where(x => x.objectId == id && x.objectType == "resourceType").ToListAsync();
      _context.TagAcls.RemoveRange(tagsAcl);

      _context.ResourceTypes.Remove(resourceType);

      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool ResourceTypeExists(long id)
    {
      return _context.ResourceTypes.Any(e => e.id == id);
    }
  }
}
