using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController(IAuthorRepository authorRepository, IBookRepository bookRepository) : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IBookRepository _bookRepository = bookRepository;

        [HttpGet]
        public async Task<ActionResult<object>> GetAuthors(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and size must be positive numbers.");
            }

            var totalAuthors = await _authorRepository.GetTotalAuthors();
            var totalPages = (int)Math.Ceiling(totalAuthors / (double)pageSize);
            var authors = await _authorRepository.GetAuthors(pageNumber, pageSize);

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalAuthors = totalAuthors,
                Authors = authors
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthorById(int id)
        {
            var author = await _authorRepository.GetAuthorById(id);
            if (author is null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Author>> AddAuthor(Author newAuthor)
        {
            if (newAuthor is null)
            {
                return BadRequest("Incorrect author data.");
            }

            await _authorRepository.AddAuthor(newAuthor);

            return CreatedAtAction(nameof(GetAuthorById), new { id = newAuthor.Id }, newAuthor);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] Author updatedAuthor)
        {
            var author = await _authorRepository.GetAuthorById(id);
            if (author is null)
                return NotFound();

            author.FirstName = updatedAuthor.FirstName;
            author.LastName = updatedAuthor.LastName;
            author.DateOfBirth = updatedAuthor.DateOfBirth;
            author.Country = updatedAuthor.Country;

            await _authorRepository.UpdateAuthor(author);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _authorRepository.GetAuthorById(id);
            if (author is null)
                return NotFound();

            await _authorRepository.DeleteAuthor(author);

            return NoContent();
        }

        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<List<Book>>> GetBooksByAuthor(int authorId)
        {
            var books = await _bookRepository.GetBooksByAuthorIdAsync(authorId);
            if (books == null || !books.Any())
            {
                return NotFound($"No books found for author with ID {authorId}.");
            }

            return Ok(books);
        }
    }
}