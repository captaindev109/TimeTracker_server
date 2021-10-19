using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class CompanyUserResponse
  {
    public User user { get; set; }
    public List<string> roles { get; set; }
  }

  public class CompanyUserRoleResponse
  {
    public User user { get; set; }
    public Boolean is_company_admin { get; set; }
    public Boolean is_company_controller { get; set; }
    public List<long> project_manager_projects { get; set; }
    public List<long> project_assistant_projects { get; set; }
    public List<long> team_lead_teams { get; set; }
    public List<long> worker_teams { get; set; }
  }
}
