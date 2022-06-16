using System;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);
            
            builder.HasOne(p => p.Thread)
                .WithMany(t => t.Posts)
                .HasForeignKey(t => t.ThreadId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(p => p.PublishDate)
                .HasConversion(d => d,d => DateTime.SpecifyKind(d, DateTimeKind.Utc))
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}