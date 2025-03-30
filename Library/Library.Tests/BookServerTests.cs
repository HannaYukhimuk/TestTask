using Library.Domain.Entities;
using Moq;
using Xunit;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _bookService = new BookService(_bookRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllBooksAsync()
    { 
        var books = new List<Book>
        {
            new Book { Id = 1, Title = "Test Book 1", ISBN = "123456", Genre = "Fiction", Description = "Description 1" },
            new Book { Id = 2, Title = "Test Book 2", ISBN = "789012", Genre = "Non-fiction", Description = "Description 2" }
        };
        _bookRepositoryMock.Setup(repo => repo.GetBooksPagedAsync(1, int.MaxValue)).ReturnsAsync(books);
         
        var result = await _bookService.GetAllBooksAsync();
         
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Test Book 1", result.First().Title);
    }


    [Fact]
    public async Task GetBookByIdAsync()
    { 
        var book = new Book { Id = 1, Title = "Test Book", ISBN = "123456", Genre = "Fiction", Description = "Test Description" };
        _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(book);
         
        var result = await _bookService.GetBookByIdAsync(1);
         
        Assert.NotNull(result);
        Assert.Equal("Test Book", result.Title);
    }

    [Fact]
    public async Task GetBookByIsbnAsync()
    { 
        var book = new Book { Id = 1, Title = "Test Book", ISBN = "123456", Genre = "Fiction", Description = "Test Description" };
        _bookRepositoryMock.Setup(repo => repo.GetByIsbnAsync("123456")).ReturnsAsync(book);
         
        var result = await _bookService.GetBookByIsbnAsync("123456");
         
        Assert.NotNull(result);
        Assert.Equal("Test Book", result.Title);
    }

    [Fact]
    public async Task AddBookAsync()
    { 
        var book = new Book { Title = "New Book", ISBN = "987654", Genre = "Fiction", Description = "New Description" };
        _bookRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Book>())).ReturnsAsync(book);
         
        var result = await _bookService.AddBookAsync(book);
         
        Assert.NotNull(result);
        Assert.Equal("New Book", result.Title);
    }

    [Fact]
    public async Task UpdateBookAsync()
    { 
        var updatedBook = new Book { Id = 1, Title = "Updated Book", ISBN = "123456", Genre = "Fiction", Description = "Updated Description" };
        _bookRepositoryMock.Setup(repo => repo.UpdateAsync(1, updatedBook)).ReturnsAsync(true);
         
        var result = await _bookService.UpdateBookAsync(1, updatedBook);
         
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteBookAsync()
    { 
        _bookRepositoryMock.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true);
         
        var result = await _bookService.DeleteBookAsync(1);
         
        Assert.True(result);
    }


    [Fact]
    public async Task BorrowBookAsync()
    { 
        var book = new Book { Id = 1, Title = "Test Book", ISBN = "123456", Genre = "Fiction", Description = "Test Description" };
        _bookRepositoryMock.Setup(repo => repo.BorrowBookAsync(1, It.IsAny<Guid>())).ReturnsAsync(book);
         
        var result = await _bookService.BorrowBookAsync(1, Guid.NewGuid());
         
        Assert.NotNull(result);
        Assert.Equal("Test Book", result.Title);
    }

    [Fact]
    public async Task ReturnBookAsync()
    { 
        var book = new Book { Id = 1, Title = "Test Book", ISBN = "123456", Genre = "Fiction", Description = "Test Description" };
        _bookRepositoryMock.Setup(repo => repo.ReturnBookAsync(1, It.IsAny<Guid>())).ReturnsAsync(book);
         
        var result = await _bookService.ReturnBookAsync(1, Guid.NewGuid());
         
        Assert.NotNull(result);
        Assert.Equal("Test Book", result.Title);
    }

}
