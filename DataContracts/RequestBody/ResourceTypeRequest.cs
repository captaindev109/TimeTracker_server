using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class CreateResourceTypeRequestRequest
  {
    public ResourceType resourceType { get; set; }
    public long companyId { get; set; }
    public List<string> tags { get; set; }
  }

  public class UpdateResourceTypeRequestRequest
  {
    public ResourceType resourceType { get; set; }
    public long companyId { get; set; }
    public List<string> tags { get; set; }
  }
}
