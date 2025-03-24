using Library.Domain.Entities;
using Library.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Library.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly LibraryService _libraryService;

        public BookController(LibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpGet]
        public ActionResult<List<Book>> GetBooks()
        {
            return Ok(_libraryService.Books);
        }

        [HttpGet("id/{id}")]
        public ActionResult<Book> GetBookById(int id)
        {
            var book = _libraryService.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpGet("{isbn}")]
        public ActionResult<Book> GetBookByIsbn(string isbn)
        {
            var book = _libraryService.Books.FirstOrDefault(b => b.ISBN == isbn);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpPost]
        public ActionResult<Book> AddBook([FromBody] Book newBook)
        {
            if (newBook == null)
            {
                return BadRequest("Некорректные данные книги.");
            }

            newBook.Id = _libraryService.Books.Any() ? _libraryService.Books.Max(b => b.Id) + 1 : 1;
            _libraryService.Books.Add(newBook);

            return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] Book updatedBook)
        {
            var book = _libraryService.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();

            book.Title = updatedBook.Title;
            book.Description = updatedBook.Description;
            book.Genre = updatedBook.Genre;
            book.Author = updatedBook.Author;
            book.ISBN = updatedBook.ISBN;

            return Ok(book);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var book = _libraryService.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();

            _libraryService.Books.Remove(book);
            return NoContent();
        }

        
    }
}

public class BorrowRequest
{
    public int DaysUntilReturn { get; set; }
}
