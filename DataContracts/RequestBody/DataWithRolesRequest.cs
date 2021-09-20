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
}
