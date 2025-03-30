using Library.Domain.Entities;
using Library.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using Library.Domain.Repositories;

namespace Library.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public BookController(IBookRepository bookRepository, IAuthorRepository authorRepository, IMemoryCache cache, IMapper mapper, IWebHostEnvironment env)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _cache = cache;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks(int pageNumber = 1, int pageSize = 10)
        {
            return Ok(await _bookRepository.GetBooksPagedAsync(pageNumber, pageSize));
        }

        [HttpGet("id/{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return book is null ? NotFound() : Ok(book);
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<Book>> GetBookIsbn(string isbn)
        {
            var book = await _bookRepository.GetByIsbnAsync(isbn);
            return book is null ? NotFound() : Ok(book);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Book>> AddBook(BookCreateDto newBookDto)
        {
            var book = _mapper.Map<Book>(newBookDto);
            book.Author = await _authorRepository.FindOrCreateAuthorAsync(newBookDto.FirstName, newBookDto.LastName);
            var createdBook = await _bookRepository.AddAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book updatedBook)
        {
            return await _bookRepository.UpdateAsync(id, updatedBook) ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            return await _bookRepository.DeleteAsync(id) ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload-image/{id}")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var result = await _bookRepository.UploadImageAsync(id, file, _env.WebRootPath);
            if (result is null) return NotFound();
            return Ok(new { ImagePath = result });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var imageBytes = await _bookRepository.GetImageAsync(id, _cache, _env.WebRootPath);
            if (imageBytes is null) return NotFound();
            return File(imageBytes, "image/jpeg");
        }

        [Authorize]
        [HttpPost("borrow/{bookId}")]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized("Unable to determine user.");

            var result = await _bookRepository.BorrowBookAsync(bookId, Guid.Parse(userId));
            if (result is null) return BadRequest("The book has already been checked out to another user or was not found.");
            return Ok(result);
        }

        [Authorize]
        [HttpPost("return/{bookId}")]
        public async Task<IActionResult> ReturnBook(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized("Unable to determine user.");

            var result = await _bookRepository.ReturnBookAsync(bookId, Guid.Parse(userId));
            if (result is null) return BadRequest("You didn't take this book or you've already returned it.");
            return Ok(result);
        }
    }
}