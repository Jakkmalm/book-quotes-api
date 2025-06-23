using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookQuotesApi.Models;
using System.Linq;
using System.Security.Claims;



namespace BookQuotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]   // kräver giltig JWT
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _db;
        public BooksController(AppDbContext db) => _db = db;

        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = GetUserId();
            var books = _db.Books.Where(b => b.AppUserId == userId).ToList();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userId = GetUserId();
            var book = _db.Books.FirstOrDefault(b => b.Id == id && b.AppUserId == userId);
            return book == null ? NotFound() : Ok(book);
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            var userId = GetUserId();
            book.AppUserId = userId;

            _db.Books.Add(book);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Book updated)
        {
            var userId = GetUserId();
            var book = _db.Books.FirstOrDefault(b => b.Id == id && b.AppUserId == userId);
            if (book == null) return NotFound();

            book.Title = updated.Title;
            book.Author = updated.Author;
            book.Published = updated.Published;
            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userId = GetUserId();
            var book = _db.Books.FirstOrDefault(b => b.Id == id && b.AppUserId == userId);
            if (book == null) return NotFound();

            _db.Books.Remove(book);
            _db.SaveChanges();

            return NoContent();
        }

        private int GetUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                throw new UnauthorizedAccessException("Token saknar NameIdentifier");
            }
            return int.Parse(userIdStr);
        }
    }
}