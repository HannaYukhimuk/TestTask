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
using Microsoft.Extensions.Caching.Memory;
using Library.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Library.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpClientFactory _httpClientFactory;


        public BookController(LibraryDbContext context, IMemoryCache cache, IWebHostEnvironment env, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _cache = cache;
            _env = env;
            _httpClientFactory = httpClientFactory;
        }

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







        [HttpPost("upload-image/{id}")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            book.ImagePath = $"/images/{file.FileName}";
            await _context.SaveChangesAsync();
            return Ok(new { book.ImagePath });
        }

        [HttpGet("image/{id}")]
        public IActionResult GetImage(int id)
        {
            if (!_cache.TryGetValue(id, out byte[] imageBytes))
            {
                var book = _context.Books.Find(id);
                if (book == null || string.IsNullOrEmpty(book.ImagePath)) return NotFound();

                var filePath = Path.Combine(_env.WebRootPath, book.ImagePath.TrimStart('/'));
                if (!System.IO.File.Exists(filePath)) return NotFound();

                imageBytes = System.IO.File.ReadAllBytes(filePath);
                _cache.Set(id, imageBytes, TimeSpan.FromMinutes(10)); // Кешируем на 10 минут
            }
            return File(imageBytes, "image/jpeg");
        }












        [Authorize]
        [HttpPost("borrow/{bookId}")]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Не удалось определить пользователя.");

            var userId = Guid.Parse(userIdClaim.Value);

            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                return NotFound("Книга не найдена.");

            var existingLoan = await _context.UserBooks
                .FirstOrDefaultAsync(bl => bl.BookId == bookId && bl.ReturnedAt == null);

            if (existingLoan != null)
                return BadRequest("Книга уже выдана другому пользователю.");

            var newLoan = new UserBook
            {
                UserId = userId,
                BookId = bookId,
                BorrowedAt = DateTime.UtcNow,
                ReturnBy = DateTime.UtcNow.AddDays(14) // 14 дней на возврат
            };

            book.BorrowedAt = DateTime.UtcNow;
            book.ReturnBy = DateTime.UtcNow.AddDays(14); // Срок возврата - 14 дней



            _context.UserBooks.Add(newLoan);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Книга успешно выдана.",
                book.Title,
                BorrowedAt = newLoan.BorrowedAt,
                ReturnBy = newLoan.ReturnBy
            });
        }

        [Authorize]
        [HttpPost("return/{bookId}")]
        public async Task<IActionResult> ReturnBook(int bookId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Не удалось определить пользователя.");

            var userId = Guid.Parse(userIdClaim.Value);

            var bookLoan = await _context.UserBooks
                .Include(bl => bl.Book) // Загружаем связанную книгу
                .FirstOrDefaultAsync(bl => bl.BookId == bookId && bl.UserId == userId && bl.ReturnedAt == null);

            if (bookLoan == null)
                return BadRequest("Вы не брали эту книгу или уже вернули.");

            if (bookLoan.Book == null)
                return BadRequest("Ошибка: Книга не найдена в базе данных.");


            var book = await _context.Books.FindAsync(bookId);
            book.BorrowedAt = null;
            book.ReturnBy = null; 

            bookLoan.ReturnedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Книга успешно возвращена.",
                BookTitle = bookLoan.Book.Title, // Безопасный доступ к названию книги
                ReturnedAt = bookLoan.ReturnedAt
            });
        }



    }
}
