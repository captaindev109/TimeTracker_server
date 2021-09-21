using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class TeamWithRoleResponse
  {
    public Team team { get; set; }
    public bool isEditable { get; set; }
    public List<User> teamLeads { get; set; }
    public List<User> members { get; set; }
  }
}
