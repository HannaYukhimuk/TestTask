using System.Collections.Generic;
using Library.Domain;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.UI.Data
{
    public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }
    }
}
