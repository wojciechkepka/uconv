using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UConv.Core
{
    public partial class ConverterDBContext: DbContext
    {
        public ConverterDBContext()
        {
        }

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
            this.Ratings.Add(rating);
        }


        public List<Record> SelectRecordsPage(int page, int count, String convFilter, DateTime? startDateFilter, DateTime? endDateFilter)
        {
            List<Record> records = new List<Record>();
            if (page >= 1)
            {
                var i = 0;

                using (var context = new ConverterDBContext())
                {
                    var selectedRecords = context.Records.AsQueryable();
                    if (convFilter != null && convFilter.ToString() != "")
                    {
                        selectedRecords = selectedRecords.Where(r => r.converter == convFilter.ToString());
                    }
                    if (startDateFilter != null)
                    {
                        selectedRecords = selectedRecords.Where(r => r.date >= startDateFilter);
                    }
                    if (endDateFilter != null)
                    {
                        selectedRecords = selectedRecords.Where(r => r.date <= endDateFilter);
                    }
                    foreach (Record r in selectedRecords.ToList())
                    {
                        if (i >= count * (page - 1))
                        {
                            bool add = true;

                            if (add)
                            {
                                records.Add(r);
                            }
                        }
                        if (records.Count == count)
                        {
                            break;
                        }
                        i++;
                    }
                }
            }

            return records;
        }
    }
}
