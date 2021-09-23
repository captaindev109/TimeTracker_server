using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker_server.Models;

namespace DataContracts.RequestBody
{
  public class ResourceTypeResponse
  {
    public ResourceType resourceType { get; set; }
    public List<Tag> tags { get; set; }
  }
}
