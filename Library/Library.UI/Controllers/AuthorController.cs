using Library.Domain;
using Library.Domain.Entities;
using Library.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly LibraryService _libraryService;

        public AuthorController(LibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpGet]
        public ActionResult<List<Author>> GetAuthors()
        {
            return Ok(_libraryService.Authors);
        }

        [HttpGet("{id}")]
        public ActionResult<Author> GetAuthorById(int id)
        {
            var author = _libraryService.Authors.FirstOrDefault(a => a.Id == id);
            if (author is null)
                return NotFound();

            return Ok(author);
        }

        [HttpPost]
        public ActionResult<Author> AddAuthor([FromBody] Author newAuthor)
        {
            if (newAuthor == null || string.IsNullOrWhiteSpace(newAuthor.FirstName) || string.IsNullOrWhiteSpace(newAuthor.LastName))
            {
                return BadRequest("Некорректные данные автора.");
            }

            newAuthor.Id = _libraryService.Authors.Any() ? _libraryService.Authors.Max(a => a.Id) + 1 : 1;
            _libraryService.Authors.Add(newAuthor);

            return CreatedAtAction(nameof(GetAuthorById), new { id = newAuthor.Id }, newAuthor);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAuthor(int id, [FromBody] Author updatedAuthor)
        {
            var author = _libraryService.Authors.FirstOrDefault(a => a.Id == id);
            if (author is null)
                return NotFound();

            if (updatedAuthor == null || string.IsNullOrWhiteSpace(updatedAuthor.FirstName) || string.IsNullOrWhiteSpace(updatedAuthor.LastName))
            {
                return BadRequest("Некорректные данные для обновления.");
            }

            author.FirstName = updatedAuthor.FirstName;
            author.LastName = updatedAuthor.LastName;
            author.DateOfBirth = updatedAuthor.DateOfBirth;
            author.Country = updatedAuthor.Country;

            return Ok(author);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(int id)
        {
            var author = _libraryService.Authors.FirstOrDefault(a => a.Id == id);
            if (author is null)
                return NotFound();

            _libraryService.Authors.Remove(author);
            return NoContent();
        }

        [HttpGet("author/{authorId}")]
        public ActionResult<List<Book>> GetBooksByAuthor(int authorId)
        {
            var booksByAuthor = _libraryService.Books.Where(b => b.Author.Id == authorId).ToList();

            if (booksByAuthor.Count == 0)
            {
                return NotFound($"No books found for author with ID {authorId}.");
            }

            return Ok(booksByAuthor);
        }
    }
}
