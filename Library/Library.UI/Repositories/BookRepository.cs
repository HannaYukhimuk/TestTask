using Library.Domain.Entities;
using Library.Domain.Models;
using Library.UI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;
using Library.Domain;

namespace Library.UI.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;
        public BookRepository(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> GetBooksPagedAsync(int pageNumber, int pageSize)
        {
            var totalBooks = await _context.Books.CountAsync();
            var totalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);

            var books = await _context.Books
                .OrderBy(b => b.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.ISBN,
                    b.Genre,
                    b.Description,
                    AuthorFirstName = b.Author.FirstName,
                    AuthorLastName = b.Author.LastName
                })
                .ToListAsync();

            return new { PageNumber = pageNumber, PageSize = pageSize, TotalPages = totalPages, TotalBooks = totalBooks, Books = books };
        }

        public async Task<object> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Where(b => b.Id == id)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.ISBN,
                    b.Genre,
                    b.Description,
                    AuthorFirstName = b.Author.FirstName,
                    AuthorLastName = b.Author.LastName
                })
                .FirstOrDefaultAsync();
        }

        public async Task<object> GetBookByIsbnAsync(string isbn)
        {
            return await _context.Books
                .Where(b => b.ISBN == isbn)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.ISBN,
                    b.Genre,
                    b.Description,
                    AuthorFirstName = b.Author.FirstName,
                    AuthorLastName = b.Author.LastName
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Book> AddBookAsync(BookCreateDto newBookDto)
        {
            var book = _mapper.Map<Book>(newBookDto); 

            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.FirstName == newBookDto.FirstName && a.LastName == newBookDto.LastName);

            if (author == null)
            {
                author = new Author
                {
                    FirstName = newBookDto.FirstName,
                    LastName = newBookDto.LastName,

                };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
            }

            book.Author = author;  

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }



        public async Task<bool> UpdateBookAsync(int id, Book updatedBook)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            book.Title = updatedBook.Title;
            book.ISBN = updatedBook.ISBN;
            book.Genre = updatedBook.Genre;
            book.Description = updatedBook.Description;
            book.BorrowedAt = updatedBook.BorrowedAt;
            book.ReturnBy = updatedBook.ReturnBy;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> UploadImageAsync(int id, IFormFile file, string webRootPath)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            var filePath = Path.Combine(webRootPath, "images", $"{id}.jpg");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            book.ImagePath = $"/images/{file.FileName}";
            await _context.SaveChangesAsync();

            return filePath;
        }

        public async Task<byte[]> GetImageAsync(int id, IMemoryCache cache, string webRootPath)
        {
            var filePath = Path.Combine(webRootPath, "images", $"{id}.jpg");
            if (!File.Exists(filePath)) return null;

            return await File.ReadAllBytesAsync(filePath);
        }

        public async Task<object> BorrowBookAsync(int bookId, Guid userId)
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

            return new { book.Id, book.Title, book.BorrowedAt, book.ReturnBy };
        }

        public async Task<object> ReturnBookAsync(int bookId, Guid userId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.BorrowedAt == null) return null;


            var bookLoan = await _context.UserBooks
    .FirstOrDefaultAsync(bl => bl.BookId == bookId && bl.UserId == userId && bl.ReturnedAt == null);

            if (bookLoan == null)
            {
                return new { Error = "Book checkout record not found or book already returned." };
            }


            book.BorrowedAt = null;
            book.ReturnBy = null;

            bookLoan.ReturnedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new { book.Id, book.Title, Message = "The book has been returned" };
        }

        public async Task<List<Book>> GetBooksByAuthorIdAsync(int authorId)
        {
            return await _context.Books
                .Where(b => b.Author.Id == authorId)
                .ToListAsync();
        }
    }
}
