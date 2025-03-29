using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class UserBook
    {
        public int Id { get; set; }

        public Guid UserId { get; set; } 

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }

        public DateTime BorrowedAt { get; set; }
        public DateTime ReturnBy { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }

}
