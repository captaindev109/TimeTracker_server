using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTableApi.Models;

namespace TimeTracker_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableController : ControllerBase
    {
        private readonly TimeTableContext _context;

        public TimeTableController(TimeTableContext context)
        {
            _context = context;
        }

        // GET: api/TimeTable
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeTable>>> GetTimeTables()
        {
            return await _context.TimeTables.ToListAsync();
        }

        // GET: api/TimeTable/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TimeTable>> GetTimeTable(long id)
        {
            var timeTable = await _context.TimeTables.FindAsync(id);

            if (timeTable == null)
            {
                return NotFound();
            }

            return timeTable;
        }

        // PUT: api/TimeTable/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeTable(long id, TimeTable timeTable)
        {
            if (id != timeTable.id)
            {
                return BadRequest();
            }

            _context.Entry(timeTable).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeTableExists(id))
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

        // POST: api/TimeTable
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TimeTable>> PostTimeTable(TimeTable timeTable)
        {
            _context.TimeTables.Add(timeTable);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTimeTable", new { id = timeTable.id }, timeTable);
        }

        // DELETE: api/TimeTable/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeTable(long id)
        {
            var timeTable = await _context.TimeTables.FindAsync(id);
            if (timeTable == null)
            {
                return NotFound();
            }

            _context.TimeTables.Remove(timeTable);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TimeTableExists(long id)
        {
            return _context.TimeTables.Any(e => e.id == id);
        }
    }
}
