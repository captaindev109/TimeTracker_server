using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts.RequestBody
{
  public class DataWithRolesRequest
  {
    public long companyId { get; set; }
    public long userId { get; set; }
    public List<string> userRoles { get; set; }
  }

  public class UpdateUserRolesRequest
  {
    public long userId { get; set; }
    public long companyId { get; set; }
    
    public Boolean is_company_admin { get; set; }
    public Boolean is_company_controller { get; set; }
    public List<long> project_manager_projects { get; set; }
    public List<long> project_assistant_projects { get; set; }
    public List<long> team_lead_teams { get; set; }
    public List<long> worker_teams { get; set; }
  }
}
