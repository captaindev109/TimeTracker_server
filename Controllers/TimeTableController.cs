using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
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
  public class TimeTableController : ControllerBase
  {
    private readonly MyDbContext _context;

    public TimeTableController(MyDbContext context)
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

    // POST: api/TimeTable/taskItem
    [HttpPost("taskItem")]
    public async Task<ActionResult<IEnumerable<TimeTable>>> GetTimeTableOfTaskItem(GetTaskItemTimeTable request)
    {
      var userId = request.userId;
      var companyId = request.companyId;
      var taskItemId = request.taskItemId;

      var timeTableIds = await _context.TaskItemAcls.Where(x => x.userId == userId && x.companyId == companyId && x.taskItemId == taskItemId).Select(x => x.timeTableId).ToListAsync();
      var timeTables = await _context.TimeTables.Where(x => timeTableIds.Contains(x.id)).ToListAsync();

      return timeTables;
    }

    // POST: api/TimeTable/current-week
    [HttpPost("current-week")]
    public async Task<ActionResult<IEnumerable<WeekTimeTableResponse>>> GetCurrentWeekTimeTable(TimeTableRequest request)
    {
      var userId = request.userId;
      var companyId = request.companyId;
      var currentWeek = request.currentWeek;

      var taskItemAcls = _context.TaskItemAcls.Where(x => x.userId == userId && x.companyId == companyId && x.status != "Progress" && x.timeTableId != -1);
      // var timeTableIds = await taskItemAcls.Select(x => x.timeTableId).ToListAsync();
      var taskItems = await taskItemAcls.ToListAsync();

      // var timeTables = await _context.TimeTables.Where(x => timeTableIds.Contains(x.id) && x.end != -1).ToListAsync();

      var res = new List<WeekTimeTableResponse>();

      foreach (var taskItemAcl in taskItems)
      {
        if (res.Find(item => item.taskItemId == taskItemAcl.taskItemId) == null)
        {
          var timeTableIds = await taskItemAcls.Where(x => x.taskItemId == taskItemAcl.taskItemId).Select(x => x.timeTableId).ToListAsync();
          var timeTables = await _context.TimeTables.Where(x => timeTableIds.Contains(x.id) && x.end != -1).ToListAsync();

          var weekLogs = new List<TimeTable>();

          foreach (var tableItem in timeTables)
          {
            var weekNumber = WeekNumOfUnixTime(tableItem.end);
            if (weekNumber == currentWeek)
            {
              weekLogs.Add(tableItem);
            }
          }

          if (weekLogs.Count > 0)
          {
            var item = new WeekTimeTableResponse();
            item.taskItemId = taskItemAcl.taskItemId;
            item.weekLogs = weekLogs;
            res.Add(item);
          }
        }
      }

      return res;
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

    private int WeekNumOfUnixTime(long unixTimeStamp)
    {
      DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
      Calendar cal = new CultureInfo("en-US").Calendar;
      int week = cal.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
      return week;
    }
  }
}
