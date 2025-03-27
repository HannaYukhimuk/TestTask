using Library.Domain;
using Library.Domain.Entities;
using Library.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Library.UI.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController(LibraryDbContext context) : ControllerBase
    {
        private readonly LibraryDbContext _context = context;


        [HttpGet]
        public async Task<ActionResult<List<Author>>> GetAuthors()
        {
            return Ok(await _context.Authors.ToListAsync());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Author>> GetAuthorById(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author is null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<Author>> AddAuthor(Author newAuthor)
        {
            if (newAuthor is null)
            {
                return BadRequest("Некорректные данные автора.");
            }

            _context.Authors.Add(newAuthor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthorById), new { id = newAuthor.Id }, newAuthor);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] Author updatedAuthor)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author is null)
                return NotFound();

            author.FirstName = updatedAuthor.FirstName;
            author.LastName = updatedAuthor.LastName;
            author.DateOfBirth = updatedAuthor.DateOfBirth;
            author.Country = updatedAuthor.Country;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author is null)
                return NotFound();

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //&&&&&&>?????????????????

        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<List<Book>>> GetBooksByAuthor(int authorId)
        {
            var booksByAuthor = _context.Books.Where(b => b.Author.Id == authorId).ToList();

            if (booksByAuthor.Count == 0)
            {
                return NotFound($"No books found for author with ID {authorId}.");
            }

            return Ok(booksByAuthor);
        }
    }
}
