using Library.Domain.Entities;
using Library.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetBooksPagedAsync(int pageNumber, int pageSize);
    Task<Book> GetByIdAsync(int id);
    Task<Book> GetByIsbnAsync(string id);
    Task<Book> AddAsync(Book book);
    Task<bool> UpdateAsync(int id, Book updatedBook);
    Task<bool> DeleteAsync(int id);
    Task<string?> UploadImageAsync(int id, IFormFile file, string webRootPath);
    Task<byte[]?> GetImageAsync(int id, IMemoryCache cache, string webRootPath);
    Task<Book?> BorrowBookAsync(int bookId, Guid userId);
    Task<Book?> ReturnBookAsync(int bookId, Guid userId);

    
}