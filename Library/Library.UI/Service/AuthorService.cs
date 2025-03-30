using Library.Domain.Entities;
using Library.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Domain.Services
{
    public class AuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;

        public AuthorService(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Author>> GetAuthorsAsync(int pageNumber, int pageSize)
        {
            return await _authorRepository.GetAuthors(pageNumber, pageSize);
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await _authorRepository.GetAuthorById(id);
        }

        public async Task AddAuthorAsync(Author newAuthor)
        {
            await _authorRepository.AddAuthor(newAuthor);
        }

        public async Task<bool> UpdateAuthorAsync(int id, Author updatedAuthor)
        {
            var author = await _authorRepository.GetAuthorById(id);
            if (author == null)
            {
                return false; // Если автор не найден, возвращаем false
            }

            author.FirstName = updatedAuthor.FirstName;
            author.LastName = updatedAuthor.LastName;
            author.DateOfBirth = updatedAuthor.DateOfBirth;
            author.Country = updatedAuthor.Country;

            await _authorRepository.UpdateAuthor(author);
            return true;
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var author = await _authorRepository.GetAuthorById(id);
            if (author == null)
            {
                return false; // Если автор не найден, возвращаем false
            }

            await _authorRepository.DeleteAuthor(author);
            return true;
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId)
        {
            var books = await _authorRepository.GetBooksByAuthorIdAsync(authorId);
            return books ?? new List<Book>(); // Возвращаем пустой список, если книги не найдены
        }
    }
}
