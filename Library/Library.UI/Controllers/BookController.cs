using Library.Domain.Entities;
using Library.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using Library.UI.Data;
using Library.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController(LibraryDbContext context) : ControllerBase
    {
        private readonly LibraryDbContext _context = context;


        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetBooks()
        {
            return Ok(await _context.Books.ToListAsync());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpGet]
        [Route("{isbn}")]
        public async Task<ActionResult<Book>> GetBookByIsbn(int isbn)
        {
            var book = await _context.Books.FindAsync(isbn);
            if (book is null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> AddBook(Book newBook)
        {
            if (newBook is null)
            {
                return BadRequest("Некорректные данные книги.");
            }

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, newBook);
        }












        //[HttpPut("{id}")]
        //public IActionResult UpdateBook(int id, [FromBody] Book updatedBook)
        //{
        //    var book = _libraryService.Books.FirstOrDefault(b => b.Id == id);
        //    if (book == null)
        //        return NotFound();

        //    book.Title = updatedBook.Title;
        //    book.Description = updatedBook.Description;
        //    book.Genre = updatedBook.Genre;
        //    book.Author = updatedBook.Author;
        //    book.ISBN = updatedBook.ISBN;

        //    return Ok(book);
        //}

        //[HttpDelete("{id}")]
        //public IActionResult DeleteBook(int id)
        //{
        //    var book = _libraryService.Books.FirstOrDefault(b => b.Id == id);
        //    if (book == null)
        //        return NotFound();

        //    _libraryService.Books.Remove(book);
        //    return NoContent();
        //}


    }
}
