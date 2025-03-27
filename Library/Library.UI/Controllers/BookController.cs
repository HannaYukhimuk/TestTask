using Library.Domain.Entities;
using Library.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using Library.UI.Data;
using Library.Domain;
using Microsoft.EntityFrameworkCore;
using Library.Domain.Models;

namespace Library.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController(LibraryDbContext context) : ControllerBase
    {
        private readonly LibraryDbContext _context = context;


        [HttpGet]
        public async Task<ActionResult<List<object>>> GetBooks()
        {
            var books = await _context.Books
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.ISBN,
                    b.Genre,
                    b.Description,
                    AuthorFirstName = b.Author.FirstName,
                    AuthorLastName = b.Author.LastName
                })
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet("id/{id:int}")]
        public async Task<ActionResult<object>> GetBookById(int id)
        {
            var book = await _context.Books
                .Where(b => b.Id == id)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.ISBN,
                    b.Genre,
                    b.Description,
                    AuthorFirstName = b.Author.FirstName,
                    AuthorLastName = b.Author.LastName
                })
                .FirstOrDefaultAsync();

            if (book is null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<object>> GetBookByIsbn(string isbn)
        {
            var book = await _context.Books
                .Where(b => b.ISBN == isbn)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.ISBN,
                    b.Genre,
                    b.Description,
                    AuthorFirstName = b.Author.FirstName,
                    AuthorLastName = b.Author.LastName
                })
                .FirstOrDefaultAsync();

            if (book is null)
            {
                return NotFound();
            }

            return Ok(book);
        }


        [HttpPost]
        public async Task<ActionResult<Book>> AddBook(BookCreateDto newBookDto)
        {
            if (newBookDto is null)
            {
                return BadRequest("Некорректные данные книги.");
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.FirstName == newBookDto.FirstName && a.LastName == newBookDto.LastName);

            if (author == null)
            {
                author = new Author
                {
                    FirstName = newBookDto.FirstName,
                    LastName = newBookDto.LastName
                };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync(); 
            }

            var newBook = new Book
            {
                ISBN = newBookDto.ISBN,
                Title = newBookDto.Title,
                Genre = newBookDto.Genre,
                Description = newBookDto.Description,
                Author = author, 
                BorrowedAt = newBookDto.BorrowedAt,
                ReturnBy = newBookDto.ReturnBy
            };

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, newBook);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book updatedBook)
        {
            var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
            if (book is null)
                return NotFound();

            book.Title = updatedBook.Title;
            book.ISBN = updatedBook.ISBN;
            book.Genre = updatedBook.Genre;
            book.Description = updatedBook.Description;
            book.BorrowedAt = updatedBook.BorrowedAt;
            book.ReturnBy = updatedBook.ReturnBy;

            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.FirstName == updatedBook.Author.FirstName && a.LastName == updatedBook.Author.LastName);

            if (author is null)
            {
                return BadRequest("Указанный автор не найден.");
            }

            book.Author = author;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }









    }
}
