using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace UConv.Core
{
    public class UConvDbContext : DbContext
    {
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }

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


        public List<Record> SelectRecordsPage(int page, int count, string convFilter, DateTime? startDateFilter,
            DateTime? endDateFilter)
        {
            var records = new List<Record>();
            if (page >= 1)
            {
                var i = 0;

                using (var context = new UConvDbContext())
                {
                    var selectedRecords = context.Records.AsQueryable();
                    if (convFilter != null && convFilter != "")
                        selectedRecords = selectedRecords.Where(r => r.converter == convFilter);
                    if (startDateFilter != null)
                        selectedRecords = selectedRecords.Where(r => r.date >= startDateFilter);
                    if (endDateFilter != null) selectedRecords = selectedRecords.Where(r => r.date <= endDateFilter);
                    foreach (var r in selectedRecords.ToList())
                    {
                        if (i >= count * (page - 1))
                        {
                            var add = true;

                            if (add) records.Add(r);
                        }

                        if (records.Count == count) break;
                        i++;
                    }
                }
            }

            return records;
        }
    }
}