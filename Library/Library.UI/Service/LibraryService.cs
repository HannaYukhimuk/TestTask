using Library.Domain;
using Library.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Library.UI.Services
{
    public class LibraryService
    {
        public List<Author> Authors { get; } = new List<Author>
        {
            new Author { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1980, 5, 10), Country = "USA" },
            new Author { Id = 2, FirstName = "Alice", LastName = "Smith", DateOfBirth = new DateTime(1990, 8, 20), Country = "Canada" },
            new Author { Id = 3, FirstName = "Bob", LastName = "Johnson", DateOfBirth = new DateTime(1975, 3, 15), Country = "UK" },
            new Author { Id = 4, FirstName = "Harper", LastName = "Lee", DateOfBirth = new DateTime(1926, 4, 28), Country = "USA" },
            new Author { Id = 5, FirstName = "Jane", LastName = "Austen", DateOfBirth = new DateTime(1775, 12, 16), Country = "UK" },
            new Author { Id = 6, FirstName = "F. Scott", LastName = "Fitzgerald", DateOfBirth = new DateTime(1896, 9, 24), Country = "USA" }
        };

        public List<Book> Books { get; } = new List<Book>();

        public LibraryService()
        {
            Books.AddRange(new List<Book>
            {
                new Book { Id = 1, ISBN = "9780061122415", Title = "To Kill a Mockingbird", Genre = "Fiction", Description = "A novel by Harper Lee",
                    Author = Authors.Find(a => a.FirstName == "Harper" && a.LastName == "Lee") },

                new Book { Id = 2, ISBN = "9780141439563", Title = "Pride and Prejudice", Genre = "Classic", Description = "A novel by Jane Austen",
                    Author = Authors.Find(a => a.FirstName == "Jane" && a.LastName == "Austen") },

                new Book { Id = 3, ISBN = "9780743273565", Title = "The Great Gatsby", Genre = "Fiction", Description = "A novel by F. Scott Fitzgerald",
                    Author = Authors.Find(a => a.FirstName == "F. Scott" && a.LastName == "Fitzgerald") }
            });
        }
    }
}