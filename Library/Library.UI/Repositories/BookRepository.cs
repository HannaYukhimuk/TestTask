using Library.Domain.Entities;
using Library.UI.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context, AutoMapper.IMapper @object)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetBooksPagedAsync(int pageNumber, int pageSize)
    {
        return await _context.Books
            .OrderBy(b => b.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Book> GetByIdAsync(int id)
    {
        return await _context.Books.FindAsync(id);
    }

    public async Task<Book> GetByIsbnAsync(string isbn)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.ISBN == isbn);  
    }


    public async Task<Book> AddAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<bool> UpdateAsync(int id, Book updatedBook)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return false;

        _context.Entry(book).CurrentValues.SetValues(updatedBook);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string?> UploadImageAsync(int id, IFormFile file, string webRootPath)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return null;

        var uploadsFolder = Path.Combine(webRootPath, "Images");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, $"{id}.jpg");
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        book.ImagePath = $"/images/{id}.jpg";
        await _context.SaveChangesAsync();

        return book.ImagePath;
    }

    public async Task<byte[]?> GetImageAsync(int id, IMemoryCache cache, string webRootPath)
    {
        var cacheKey = $"BookImage_{id}";
        if (cache.TryGetValue(cacheKey, out byte[]? imageBytes))
        {
            return imageBytes;
        }

        var filePath = Path.Combine(webRootPath, "Images", $"{id}.jpg");
        if (!File.Exists(filePath)) return null;

        imageBytes = await File.ReadAllBytesAsync(filePath);
        cache.Set(cacheKey, imageBytes, TimeSpan.FromMinutes(10));

        return imageBytes;
    }

    public async Task<Book?> BorrowBookAsync(int bookId, Guid userId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null || book.BorrowedAt != null) return null;

        var newLoan = new UserBook
        {
            UserId = userId,
            BookId = bookId,
            BorrowedAt = DateTime.UtcNow,
            ReturnBy = DateTime.UtcNow.AddDays(14) // 14 дней на возврат
        };


        _context.UserBooks.Add(newLoan);

        book.BorrowedAt = DateTime.UtcNow;
        book.ReturnBy = DateTime.UtcNow.AddDays(14);
        await _context.SaveChangesAsync();

        return book;
    }


    public async Task<Book?> ReturnBookAsync(int bookId, Guid userId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null || book.BorrowedAt == null) return null;


        var bookLoan = await _context.UserBooks
    .FirstOrDefaultAsync(bl => bl.BookId == bookId && bl.UserId == userId && bl.ReturnedAt == null);

        if (bookLoan == null)
        {
            return null;
        }


        book.BorrowedAt = null;
        book.ReturnBy = null;

        _context.UserBooks.Remove(bookLoan);

        await _context.SaveChangesAsync();

        return book;
    }

    

}







