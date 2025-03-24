using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library.Domain.Entities;
using Library.Domain;

namespace Library.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        static private List<Author> _author = new List<Author>
        {
            new Author
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1980, 5, 10),
                Country = "USA"
            },
            new Author
            {
                Id = 2,
                FirstName = "Alice",
                LastName = "Smith",
                DateOfBirth = new DateTime(1990, 8, 20),
                Country = "Canada"
            },
            new Author
            {
                Id = 3,
                FirstName = "Bob",
                LastName = "Johnson",
                DateOfBirth = new DateTime(1975, 3, 15),
                Country = "UK"
            }
        };


        [HttpGet]
        public ActionResult<List<Author>> GetAuthor()
        {
            return Ok(_author);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Author> GetAuthorById(int id)
        {
            var game = _author.FirstOrDefault(g => g.Id == id);
            if (game is null)
            {
                return NotFound();
            }

            return Ok(game);
        }

        [HttpPost]
        public ActionResult<Author> AddAuthor(Author newAuthor)
        {
            if (newAuthor is null)
            {
                return BadRequest();
            }

            newAuthor.Id = _author.Max(g => g.Id) + 1;
            _author.Add(newAuthor);
            return CreatedAtAction(nameof(GetAuthorById), new { id = newAuthor.Id }, newAuthor);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAuthor(int id, Author updatedAuthor)
        {
            var author = _author.FirstOrDefault(g => g.Id == id);
            if (author is null)
                return NotFound();

            author.FirstName = updatedAuthor.FirstName;
            author.LastName = updatedAuthor.LastName;
            author.DateOfBirth = updatedAuthor.DateOfBirth;
            author.Country = updatedAuthor.Country;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(int id)
        {
            var author = _author.FirstOrDefault(g => g.Id == id);
            if (author is null)
                return NotFound();

            _author.Remove(author);
            return NoContent();
        }
    }
}
