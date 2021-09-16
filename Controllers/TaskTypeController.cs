using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker_server.Models;
using TimeTracker_server.Data;

namespace TimeTracker_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskTypeController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TaskTypeController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/TaskType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskType>>> GetTaskTypes()
        {
            return await _context.TaskTypes.ToListAsync();
        }

        // GET: api/TaskType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskType>> GetTaskType(long id)
        {
            var taskType = await _context.TaskTypes.FindAsync(id);

            if (taskType == null)
            {
                return NotFound();
            }

            return taskType;
        }

        // PUT: api/TaskType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskType(long id, TaskType taskType)
        {
            if (id != taskType.id)
            {
                return BadRequest();
            }

            _context.Entry(taskType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskTypeExists(id))
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

        // POST: api/TaskType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TaskType>> PostTaskType(TaskType taskType)
        {
            _context.TaskTypes.Add(taskType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskType", new { id = taskType.id }, taskType);
        }

        // DELETE: api/TaskType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskType(long id)
        {
            var taskType = await _context.TaskTypes.FindAsync(id);
            if (taskType == null)
            {
                return NotFound();
            }

            _context.TaskTypes.Remove(taskType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskTypeExists(long id)
        {
            return _context.TaskTypes.Any(e => e.id == id);
        }
    }
}
