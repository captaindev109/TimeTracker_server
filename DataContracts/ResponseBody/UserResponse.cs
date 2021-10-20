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
    public Boolean is_project_manager { get; set; }
    public Boolean is_project_assistant { get; set; }
    public Boolean is_team_lead { get; set; }
    public Boolean is_worker { get; set; }
   
  }
}
