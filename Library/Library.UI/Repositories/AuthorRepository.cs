using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Repositories;
using Library.UI.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.UI.Repositories
{
    public class AuthorRepository(LibraryDbContext context) : IAuthorRepository
    {
        private readonly LibraryDbContext _context = context;

        public async Task<IEnumerable<Author>> GetAuthors(int pageNumber, int pageSize)
        {
            return await _context.Authors
                .OrderBy(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Author?> GetAuthorById(int id)
        {
            return await _context.Authors.FindAsync(id);
        }

        public async Task AddAuthor(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAuthor(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAuthor(Author author)
        {
            var books = await _context.Books.Where(b => b.Author.Id == author.Id).ToListAsync();

            _context.Books.RemoveRange(books);  
            _context.Authors.Remove(author);    
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalAuthors()
        {
            return await _context.Authors.CountAsync();
        }

        public async Task<Author> FindOrCreateAuthorAsync(string firstName, string lastName)
        {
            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.FirstName == firstName && a.LastName == lastName);

            if (author == null)
            {
                author = new Author { FirstName = firstName, LastName = lastName };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
            }

            return author;
        }

        public async Task<List<Book>> GetBooksByAuthorIdAsync(int authorId)
        {
            var books = await _context.Books
                .Where(b => b.Author.Id == authorId)
                .ToListAsync();

            return books;  
        }

    }
}
