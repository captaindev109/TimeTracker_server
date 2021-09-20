using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class SignupRequest
  {
    public string email { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string password { get; set; }
  }
}
