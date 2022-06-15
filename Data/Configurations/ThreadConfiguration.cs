using System;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class ThreadConfiguration : IEntityTypeConfiguration<Thread>
    {
        public void Configure(EntityTypeBuilder<Thread> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Author)
                .WithMany(u => u.Threads)
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(t => t.Closed)
                .HasDefaultValue(false);
            
            builder.Property(p => p.CreationDate)
                .HasConversion(d => d,d => DateTime.SpecifyKind(d, DateTimeKind.Utc))
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}