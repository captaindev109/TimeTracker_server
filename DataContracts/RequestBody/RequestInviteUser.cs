using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts.RequestBody
{
  public class RequestInviteUser
  {
    public string email { get; set; }
    public List<long> objectId { get; set; }
    public string objectType { get; set; }
    public string role { get; set; }
    public long companyId { get; set; }
  }
}
