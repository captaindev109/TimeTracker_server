using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceTypeApi.Models;

namespace TimeTracker_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceTypeController : ControllerBase
    {
        private readonly ResourceTypeContext _context;

        public ResourceTypeController(ResourceTypeContext context)
        {
            _context = context;
        }

        // GET: api/ResourceType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceType>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        // GET: api/ResourceType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceType>> GetResourceType(Guid id)
        {
            var resourceType = await _context.Projects.FindAsync(id);

            if (resourceType == null)
            {
                return NotFound();
            }

            return resourceType;
        }

        // PUT: api/ResourceType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResourceType(Guid id, ResourceType resourceType)
        {
            if (id != resourceType.id)
            {
                return BadRequest();
            }

            _context.Entry(resourceType).State = EntityState.Modified;

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
        public async Task<ActionResult<ResourceType>> PostResourceType(ResourceType resourceType)
        {
            _context.Projects.Add(resourceType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResourceType", new { id = resourceType.id }, resourceType);
        }

        // DELETE: api/ResourceType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResourceType(Guid id)
        {
            var resourceType = await _context.Projects.FindAsync(id);
            if (resourceType == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(resourceType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResourceTypeExists(Guid id)
        {
            return _context.Projects.Any(e => e.id == id);
        }
    }
}
