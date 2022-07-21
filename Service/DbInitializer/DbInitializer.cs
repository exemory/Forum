using System;
using System.Threading.Tasks;
using Bogus;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;

namespace Service.DbInitializer
{
    /// <inheritdoc />
    public class DbInitializer : IDbInitializer
    {
        private readonly ForumContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        /// <summary>
        /// Static constructor for initializing <see cref="DbInitializer"/> class
        /// </summary>
        static DbInitializer()
        {
            // Sets randomizer seed to generate repeatable data sets
            Randomizer.Seed = new Random(0x25592ca3);
        }

        /// <summary>
        /// Constructor for initializing a <see cref="DbInitializer"/> class instance
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="userManager">Identity user manager</param>
        /// <param name="roleManager">Identity role manager</param>
        public DbInitializer(ForumContext context, UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeAsync(bool seedTestData)
        {
            await _context.Database.MigrateAsync();

            await SeedDataAsync();

            if (seedTestData)
            {
                await SeedTestDataAsync();
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Seeds required data
        /// </summary>
        private async Task SeedDataAsync()
        {
            foreach (var role in Data.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            if (!await _context.Users.AnyAsync(u => u.Id == Data.AdminUser.Id))
            {
                await _userManager.CreateAsync(Data.AdminUser, Data.AdminPassword);
                await _userManager.AddToRoleAsync(Data.AdminUser, "Administrator");
            }
        }

        /// <summary>
        /// Seeds test data
        /// </summary>
        private async Task SeedTestDataAsync()
        {
            foreach (var user in TestData.Users)
            {
                if (!await _context.Users.AnyAsync(u => u.Id == user.Id))
                {
                    await _userManager.CreateAsync(user, TestData.UserPassword);
                    await _userManager.AddToRoleAsync(user, TestData.RandomRole);
                }
            }

            foreach (var thread in TestData.Threads)
            {
                if (!await _context.Threads.AnyAsync(t => t.Id == thread.Id))
                {
                    _context.Threads.Add(thread);
                }
            }

            foreach (var post in TestData.Posts)
            {
                if (!await _context.Posts.AnyAsync(p => p.Id == post.Id))
                {
                    _context.Posts.Add(post);
                }
            }
        }
    }
}