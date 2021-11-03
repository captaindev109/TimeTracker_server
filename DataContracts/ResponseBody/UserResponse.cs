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
    public string status { get; set; }
  }

  public class CompanyUserRoleResponse
  {
    public User user { get; set; }
    public List<string> roles { get; set; }
  }

  public class UserRoleForAdminResponse
  {
    public User user { get; set; }
    public List<UserRolesOfCompay> roleDetail { get; set; }
  }

  public class UserRolesOfCompay
  {
    public Company company { get; set; }
    public List<string> roles { get; set; }
  }
}
