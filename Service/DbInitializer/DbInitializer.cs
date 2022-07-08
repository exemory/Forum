using System;
using System.Threading.Tasks;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Service.Interfaces;

namespace Service.DbInitializer
{
    /// <inheritdoc />
    public class DbInitializer : IDbInitializer
    {
        private readonly IHostEnvironment _env;
        private readonly ForumContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        /// <summary>
        /// Constructor for initializing a <see cref="DbInitializer"/> class instance/>
        /// </summary>
        /// <param name="env">Application environment</param>
        /// <param name="context">Database context</param>
        /// <param name="userManager">Identity user manager</param>
        /// <param name="roleManager">Identity role manager</param>
        public DbInitializer(IHostEnvironment env, ForumContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _env = env;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            await _context.Database.MigrateAsync();

            await SeedDataAsync();

            if (_env.IsDevelopment())
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

            var isAdminExists = await _userManager.FindByIdAsync(Data.AdminUser.Id.ToString()) != null;
            if (!isAdminExists)
            {
                await _userManager.CreateAsync(Data.AdminUser, Data.AdminPassword);
                await _userManager.AddToRoleAsync(Data.AdminUser, "Administrator");
            }
        }
        
        /// <summary>
        /// Seeds test data when application running in development environment
        /// </summary>
        private async Task SeedTestDataAsync()
        {
            foreach (var user in TestData.Users)
            {
                var isUserExists = await _userManager.FindByIdAsync(user.Id.ToString()) != null;
                if (!isUserExists)
                {
                    await _userManager.CreateAsync(user, TestData.UserPassword);
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }
            
            foreach (var thread in TestData.Threads)
            {
                if (!await _context.Threads.AnyAsync(c => c.Id == thread.Id))
                {
                    _context.Threads.Add(thread);
                }
            }
            
            foreach (var post in TestData.Posts)
            {
                if (!await _context.Posts.AnyAsync(c => c.Id == post.Id))
                {
                    _context.Posts.Add(post);
                }
            }
        }
    }
}