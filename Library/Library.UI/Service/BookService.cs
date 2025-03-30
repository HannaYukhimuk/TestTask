using Library.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetBooksPagedAsync(1, int.MaxValue);
    }

    public async Task<Book> GetBookByIdAsync(int id)
    {
        return await _bookRepository.GetByIdAsync(id);
    }

    public async Task<Book> GetBookByIsbnAsync(string isbn)
    {
        return await _bookRepository.GetByIsbnAsync(isbn);
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        return await _bookRepository.AddAsync(book);
    }

    public async Task<bool> UpdateBookAsync(int id, Book book)
    {
        return await _bookRepository.UpdateAsync(id, book);
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        return await _bookRepository.DeleteAsync(id);
    }

    public async Task<Book?> BorrowBookAsync(int bookId, Guid userId)
    {
        return await _bookRepository.BorrowBookAsync(bookId, userId);
    }

    public async Task<Book?> ReturnBookAsync(int bookId, Guid userId)
    {
        return await _bookRepository.ReturnBookAsync(bookId, userId);
    }
}
