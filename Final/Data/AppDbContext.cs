﻿using Final.Models;
using Microsoft.EntityFrameworkCore;

namespace Final.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Meet> Meets { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        }

        public DbSet<Meet> meets { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User { Id = 1, Login = "Daniil", Password = "Ldzk4Uai7Y8LKtylgV7nlqBrBZ2R/KPh+/W/8QKnYlU=" });  //Пароль 12345         
        }
    }
}
