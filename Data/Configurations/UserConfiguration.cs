using System;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(p => p.RegistrationDate)
                .HasConversion(d => d,d => DateTime.SpecifyKind(d, DateTimeKind.Utc))
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}