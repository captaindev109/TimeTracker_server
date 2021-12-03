using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class ProjectWithRoleResponse
  {
    public Project project { get; set; }
    public bool isEditable { get; set; }
    public List<User> projectManagers { get; set; }
    public List<User> projectManagerAssistants { get; set; }
    public List<Team> teams { get; set; }
    public List<Tag> tags { get; set; }
    public Kpi kpis { get; set; }
  }

  public class Kpi
  {
    public long totalTime { get; set; }
    public long currentMonthTime { get; set; }
  }
}
