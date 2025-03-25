using System.Collections.Generic;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Identity.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
