using System;
using System.Collections.Generic;

namespace TimeTracker_server.Models
{
  public class Project
  {
    public long id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public string publicStatus { get; set; }
    public DateTime planStart { get; set; }
    public DateTime planEnd { get; set; }
    public long createdBy { get; set; }
    public long updatedBy { get; set; }
    public DateTime create_timestamp { get; set; }
    public DateTime update_timestamp { get; set; }
  }

  public enum ProjectStatus
  {
    Active,
    Locked,
    AwaitEmailConfirmation
  }
}