using System;
using Data.Configurations;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ForumContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ForumContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<Post> Posts { get; set; }
        public DbSet<Thread> Threads { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new PostConfiguration());
            builder.ApplyConfiguration(new ThreadConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
        }
    }
}