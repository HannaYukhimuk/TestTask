using Library.Domain.Entities;
using Library.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

public interface IBookRepository
{
    Task<object> GetBooksPagedAsync(int pageNumber, int pageSize);
    Task<object> GetBookByIdAsync(int id);

    Task<object> GetBookByIsbnAsync(string id);
    Task<Book> AddBookAsync(BookCreateDto newBookDto);
    Task<bool> UpdateBookAsync(int id, Book updatedBook);
    Task<bool> DeleteBookAsync(int id);
    Task<string> UploadImageAsync(int id, IFormFile file, string webRootPath);
    Task<byte[]> GetImageAsync(int id, IMemoryCache cache, string webRootPath);
    Task<object> BorrowBookAsync(int bookId, Guid userId);
    Task<object> ReturnBookAsync(int bookId, Guid userId);
    Task<List<Book>> GetBooksByAuthorIdAsync(int authorId);
}