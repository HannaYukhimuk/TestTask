using System.Collections.Generic;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Identity.Data
{
    public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
    {
        public DbSet<Book> Books { get; set; }
    }
}
