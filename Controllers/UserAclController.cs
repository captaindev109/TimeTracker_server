using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAclApi.Models;

namespace TimeTracker_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAclController : ControllerBase
    {
        private readonly UserAclContext _context;

        public UserAclController(UserAclContext context)
        {
            _context = context;
        }

        // GET: api/UserAcl
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAcl>>> GetUserAcls()
        {
            return await _context.UserAcls.ToListAsync();
        }

        // GET: api/UserAcl/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAcl>> GetUserAcl(long id)
        {
            var userAcl = await _context.UserAcls.FindAsync(id);

            if (userAcl == null)
            {
                return NotFound();
            }

            return userAcl;
        }

        // PUT: api/UserAcl/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAcl(long id, UserAcl userAcl)
        {
            if (id != userAcl.id)
            {
                return BadRequest();
            }

            _context.Entry(userAcl).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAclExists(id))
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

        // POST: api/UserAcl
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserAcl>> PostUserAcl(UserAcl userAcl)
        {
            _context.UserAcls.Add(userAcl);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserAcl", new { id = userAcl.id }, userAcl);
        }

        // DELETE: api/UserAcl/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAcl(long id)
        {
            var userAcl = await _context.UserAcls.FindAsync(id);
            if (userAcl == null)
            {
                return NotFound();
            }

            _context.UserAcls.Remove(userAcl);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserAclExists(long id)
        {
            return _context.UserAcls.Any(e => e.id == id);
        }
    }
}
