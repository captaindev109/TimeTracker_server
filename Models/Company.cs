using System;

namespace CompanyApi.Models
{
  public class Company
  {
    public long id { get; set; }
    public string name { get; set; }
    public long user { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }
}