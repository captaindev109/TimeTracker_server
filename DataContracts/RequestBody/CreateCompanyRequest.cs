using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts.RequestBody
{
  public class CreateCompanyRequest
  {
    public string companyName { get; set; }
    public long userId { get; set; }
  }
}
