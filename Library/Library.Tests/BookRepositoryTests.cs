using Library.Domain.Entities;
using Library.UI.Repositories;
using Library.UI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using Moq;

public class BookRepositoryTests
{
    private readonly BookRepository _bookRepository;
    private readonly LibraryDbContext _context;

    public BookRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new LibraryDbContext(options);

        var mapperMock = new Mock<IMapper>();
        _bookRepository = new BookRepository(_context, mapperMock.Object);
    }

    [Fact]
    public async Task AddBookAsync()
    {
        var book = new Book { Id = 1, Title = "Test Book", ISBN = "123456", Genre = "Fiction", Description = "Test Description" };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        var addedBook = await _context.Books.FindAsync(1);

        Xunit.Assert.NotNull(addedBook);
        Xunit.Assert.Equal("Test Book", addedBook.Title);
    }

    [Fact]
    public async Task GetBookByIdAsync()
    {
        var book = new Book { Id = 2, Title = "Another Book", ISBN = "789012", Genre = "Non-fiction", Description = "Another Description" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var retrievedBook = await _bookRepository.GetByIdAsync(2);

        Xunit.Assert.NotNull(retrievedBook);
    }
}
