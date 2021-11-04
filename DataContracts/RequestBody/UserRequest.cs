using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class UpdateUserStatusRequest
  {
    public long companyId { get; set; }
    public long userId { get; set; }
    public string status { get; set; }
  }

  public class GetUserStatusRequest
  {
    public long companyId { get; set; }
    public long userId { get; set; }
  }

  public class UpdateUserRolesForAdminRequest
  {
    public long userId { get; set; }
    public List<UserRoleDetail> roles { get; set; }
  }

  public class UserRoleDetail
  {
    public long companyId { get; set; }
    public List<string> roles { get; set; }
  }
}
