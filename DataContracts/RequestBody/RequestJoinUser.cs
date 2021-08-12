using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts.RequestBody
{
  public class RequestJoinUser
  {
    public string userName { get; set; }
    public string email { get; set; }
    public long userId { get; set; }
    public string ownerEmail { get; set; }

  }
}
