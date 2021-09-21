using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class CreateProjectRequest
  {
    public Project project { get; set; }
    public long companyId { get; set; }
    public List<long> projectManagers { get; set; }
    public List<long> projectManagerAssistants { get; set; }
    public List<long> teams { get; set; }
  }

  public class UpdateProjectRequest
  {
    public Project project { get; set; }
    public List<long> projectManagers { get; set; }
    public List<long> projectManagerAssistants { get; set; }
    public List<long> teams { get; set; }
  }
}
