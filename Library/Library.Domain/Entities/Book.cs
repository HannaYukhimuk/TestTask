using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class Book
    {
        [Key]
        public string ISBN { get; set; }

        //[Required]
        public string Title { get; set; }

        public string Genre { get; set; }

        public string Description { get; set; }

        [ForeignKey("AuthorId")]
        public Author Author { get; set; }

        public DateTime? BorrowedAt { get; set; }

        public DateTime? ReturnBy { get; set; }
    }
}
