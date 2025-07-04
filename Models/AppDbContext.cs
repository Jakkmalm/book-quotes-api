﻿using Microsoft.EntityFrameworkCore;
using System;

namespace BookQuotesApi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts)
            : base(opts)
        { }
        public DbSet<AppUser> Users { get; set; }

        public DbSet<Book> Books { get; set; }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime Published { get; set; }

        // Koppling till AppUser
        public int AppUserId { get; set; }          // Foreign key
        public AppUser? AppUser { get; set; }       // Navigation property
    }
}
