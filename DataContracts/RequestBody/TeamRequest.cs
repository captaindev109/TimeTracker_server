using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class CreateTeamRequest
  {
    public Team team { get; set; }
    public long companyId { get; set; }
    public List<long> teamLeads { get; set; }
    public List<long> members { get; set; }
  }

  public class UpdateTeamRequest
  {
    public Team team { get; set; }
    public List<long> teamLeads { get; set; }
    public List<long> members { get; set; }
  }
}
