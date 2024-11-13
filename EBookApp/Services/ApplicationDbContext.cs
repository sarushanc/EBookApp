﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EBookApp.Models;

namespace EBookApp.Services
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Book> Book { get; set; } = default!;
        public DbSet<CartItem> CartItem { get; set; } = default!;
    }
}
