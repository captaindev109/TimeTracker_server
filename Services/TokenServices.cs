using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TimeTracker_server.Data;
using TimeTracker_server.Models;

namespace TimeTracker_server.Services
{
  public static class TokenService
  {
    public static string GenerateWebToken(ClaimsIdentity subject)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Secret));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = subject,
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = credentials,
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
    }

    public static ClaimsPrincipal ReadWebToken(string token)
    {
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Secret));

      var validationParams = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = securityKey,
        ValidateIssuer = false,
        ValidateAudience = false
      };
      return new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out SecurityToken validToken);
    }
  }
}
