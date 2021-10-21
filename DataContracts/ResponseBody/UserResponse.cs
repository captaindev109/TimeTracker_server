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
    public List<string> roles { get; set; }
  }
}
