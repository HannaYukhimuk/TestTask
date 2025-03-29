using Library.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using Library.UI.Data;
using Library.Domain;
using Microsoft.EntityFrameworkCore;
using Library.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Library.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Library.Domain.Repositories;

namespace Library.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _env;

        public BookController(IBookRepository bookRepository, IAuthorRepository authorRepository, IMemoryCache cache, IWebHostEnvironment env)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _cache = cache;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<List<object>>> GetBooks(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and size must be positive numbers.");
            }

            var response = await _bookRepository.GetBooksPagedAsync(pageNumber, pageSize);
            return Ok(response);
        }

        [HttpGet("id/{id:int}")]
        public async Task<ActionResult<object>> GetBookById(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book is null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpGet("id/{isbn}")]
        public async Task<ActionResult<object>> GetBookByIsbn(string isbn)
        {
            var book = await _bookRepository.GetBookByIsbnAsync(isbn);
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
                return BadRequest("Incorrect book data.");
            }

            var newBook = await _bookRepository.AddBookAsync(newBookDto);
            return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book updatedBook)
        {
            var success = await _bookRepository.UpdateBookAsync(id, updatedBook);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var success = await _bookRepository.DeleteBookAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }




        [HttpPost("upload-image/{id}")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var result = await _bookRepository.UploadImageAsync(id, file, _env.WebRootPath);
            if (result is null) return NotFound();
            return Ok(new { ImagePath = result });
        }

        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var imageBytes = await _bookRepository.GetImageAsync(id, _cache, _env.WebRootPath);
            if (imageBytes is null) return NotFound();
            return File(imageBytes, "image/jpeg");
        }

        [Authorize]
        [HttpPost("borrow/{bookId}")]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized("Unable to determine user.");

            var result = await _bookRepository.BorrowBookAsync(bookId, Guid.Parse(userId));
            if (result is null) return BadRequest("The book has already been checked out to another user or was not found.");
            return Ok(result);
        }

        [Authorize]
        [HttpPost("return/{bookId}")]
        public async Task<IActionResult> ReturnBook(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized("Unable to determine user.");

            var result = await _bookRepository.ReturnBookAsync(bookId, Guid.Parse(userId));
            if (result is null) return BadRequest("You didn't take this book or you've already returned it.");
            return Ok(result);
        }
    }
}