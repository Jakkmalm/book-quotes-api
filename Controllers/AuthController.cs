using System;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace BookQuotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        public AuthController(IConfiguration cfg) => _cfg = cfg;

        public record LoginDto(string Username, string Password);

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            // TODO: Här validerar du användaruppgifter mot DB eller hårdkodat
            if (dto.Username != "user" || dto.Password != "pass")
                return Unauthorized();

            // Skapa signeringscredentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Bygg token
            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: new[] { new Claim(ClaimTypes.Name, dto.Username) },
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_cfg["Jwt:DurationInMinutes"])),
                signingCredentials: creds
            );

            // Returnera den serialiserade token
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
