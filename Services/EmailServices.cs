using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Threading.Tasks;
using TimeTracker_server.Data;
using TimeTracker_server.Models;

namespace TimeTracker_server.Services
{
  public static class EmailService
  {
    public static async Task SendEmail(string to, string subject, string body)
    {
      try
      {
        SmtpClient client = new SmtpClient("robot@t22.tools");
        client.UseDefaultCredentials = false;
        client.EnableSsl = true;
        client.Port = 587;
        client.Host = "host212.checkdomain.de";
        client.Credentials = new NetworkCredential("robot@t22.tools", "?T6D2e#r0%p?mA4G");

        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress("robot@t22.tools", "FeldeIt TimeTracker");
        mailMessage.To.Add(to);
        mailMessage.Body = body;
        mailMessage.IsBodyHtml = true;
        mailMessage.Subject = subject;
        await client.SendMailAsync(mailMessage);

      }
      catch (Exception ex)
      {
        throw ex;
      }
    }
  }
}
