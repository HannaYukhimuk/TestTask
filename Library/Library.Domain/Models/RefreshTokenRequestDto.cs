using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 
    namespace Library.Domain.Models
    {
        public class RefreshTokenRequestDto
        {
            public Guid UserId { get; set; }
            public string RefreshToken { get; set; }
        }
    }
   
