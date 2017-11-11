using System;
using Microsoft.EntityFrameworkCore;
using PicBook.Domain;

namespace PicBook.Repository.EntityFramework
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}