using System;

namespace UserApi.Models
{
  public class User
  {
    public long id { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string avatar { get; set; }
    public string status { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum UserStatus
  {
    Active,
    Locked,
    AwaitEmailConfirmation
  }
}