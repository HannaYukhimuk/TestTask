using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Models
{
    public class BookCreateDto
    {
        public string ISBN { get; set; }

        [Required]
        public string Title { get; set; }

        public string Genre { get; set; }

        public string Description { get; set; }

        [Required]
        public string FirstName { get; set; } 

        [Required]
        public string LastName { get; set; } 

    }
}

