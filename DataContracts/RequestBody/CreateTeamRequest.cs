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
  }
}
