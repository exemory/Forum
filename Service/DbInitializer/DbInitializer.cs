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
    public class DbInitializer : IDbInitializer
    {
        private readonly IHostEnvironment _env;
        private readonly ForumContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

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

            await SeedData();

            if (_env.IsDevelopment())
            {
                await SeedTestData();
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedData()
        {
            foreach (var role in Data.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            var isAdminExists = await _userManager.FindByNameAsync(Data.AdminUser.UserName) != null;
            if (!isAdminExists)
            {
                await _userManager.CreateAsync(Data.AdminUser, Data.AdminPassword);
                await _userManager.AddToRoleAsync(Data.AdminUser, "Administrator");
            }
        }
        
        private async Task SeedTestData()
        {
            foreach (var user in TestData.Users)
            {
                var isUserExists = await _userManager.FindByNameAsync(user.UserName) != null;
                if (!isUserExists)
                {
                    await _userManager.CreateAsync(user, TestData.UserPassword);
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }
            
            foreach (var user in TestData.Moderators)
            {
                var isUserExists = await _userManager.FindByNameAsync(user.UserName) != null;
                if (!isUserExists)
                {
                    await _userManager.CreateAsync(user, TestData.ModerPassword);
                    await _userManager.AddToRoleAsync(user, "Moderator");
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