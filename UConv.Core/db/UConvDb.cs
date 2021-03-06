﻿using Microsoft.EntityFrameworkCore;

namespace UConv.Core.Db
{
    public class UConvDbContext : DbContext
    {
        protected readonly string dbServerConnString;

        public UConvDbContext()
        {
            dbServerConnString =
                @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UConv;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        }

        public UConvDbContext(string server)
        {
            dbServerConnString = server;
        }

        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(dbServerConnString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Record>()
                .Property(e => e.converter)
                .IsUnicode(false);

            modelBuilder.Entity<Record>()
                .Property(e => e.inputUnit)
                .IsUnicode(false);

            modelBuilder.Entity<Record>()
                .Property(e => e.outputUnit)
                .IsUnicode(false);

            modelBuilder.Entity<Rating>()
                .Property(e => e.name)
                .IsUnicode(false);
        }

        public void AddRating(Rating rating)
        {
            Ratings.Add(rating);
        }
    }
}