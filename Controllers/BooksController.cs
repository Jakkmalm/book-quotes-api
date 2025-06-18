using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookQuotesApi.Models;
using System.Linq;



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
        public IActionResult GetAll() =>
            Ok(_db.Books.ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var book = _db.Books.Find(id);
            return book == null ? NotFound() : Ok(book);
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            _db.Books.Add(book);
            _db.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Book updated)
        {
            var book = _db.Books.Find(id);
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
            var book = _db.Books.Find(id);
            if (book == null) return NotFound();
            _db.Books.Remove(book);
            _db.SaveChanges();
            return NoContent();
        }
    }
}