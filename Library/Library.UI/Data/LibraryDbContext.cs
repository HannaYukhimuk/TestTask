using System;
using Microsoft.EntityFrameworkCore;
using Library.Domain.Entities;
using Library.Domain;

namespace Library.UI.Data
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserBook>()
                .HasOne(ub => ub.Book)
                .WithMany()
                .HasForeignKey(ub => ub.BookId);
            modelBuilder.Entity<Book>()
                    .HasOne(b => b.Author)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Cascade);
        }


    }
}