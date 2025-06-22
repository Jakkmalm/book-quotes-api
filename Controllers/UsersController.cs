using Microsoft.AspNetCore.Mvc;
using BookQuotesApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace BookQuotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        public record RegisterDto(string Username, string Password);

        [HttpPost("register")]
        [AllowAnonymous]  //registrera sig utan JWT
        // PROVA ta bort AllowANonymous och testa......................senare.
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (_db.Users.Any(u => u.Username == dto.Username))
                return Conflict("Username already exists");

            var user = new AppUser
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(new { message = "User created" });
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _db.Users.Select(u => new { u.Id, u.Username, u.Role }).ToList();
            return Ok(users);
        }
    }
}
