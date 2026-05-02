using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TaskFlow2.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateToken(IConfiguration config, string userId, string email, string role = "User")
        {
            var key = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
            var issuer = config["Jwt:Issuer"];
            var audience = config["Jwt:Audience"];
            var durationMinutes = int.TryParse(config["Jwt:DurationMinutes"], out var m) ? m : 60;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(durationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}