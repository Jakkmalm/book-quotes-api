using Microsoft.AspNetCore.Mvc;
using BookQuotesApi.Models;
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

        [HttpPost("register")]
        public IActionResult Register(string username, string password)
        {
            if (_db.Users.Any(u => u.Username == username))
                return Conflict("Username already exists");

            var user = new AppUser
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok("User created");
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _db.Users.Select(u => new { u.Id, u.Username, u.Role }).ToList();
            return Ok(users);
        }
    }
}
