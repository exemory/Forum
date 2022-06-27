using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;
using Data.Repositories;
using FluentAssertions;
using Xunit;

namespace Data.Tests
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _sut;
        private readonly ForumContext _context = new ForumContext(UnitTestHelper.GetUnitTestDbOptions());

        public UserRepositoryTests()
        {
            _sut = new UserRepository(_context);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            var expected = UserList;

            var result = await _sut.GetAllAsync();

            result.Should().BeEquivalentTo(expected, o =>
                o.Excluding(u => u.ConcurrencyStamp));
        }

        private static IEnumerable<User> UserList =>
            new List<User>
            {
                new User
                {
                    Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    UserName = "username1",
                    Email = "email1@example.com",
                    Name = "name1",
                    RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
                },
                new User
                {
                    Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    UserName = "username2",
                    Email = "email2@example.com",
                    Name = "name2",
                    RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
                },
                new User
                {
                    Id = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                    UserName = "username3",
                    Email = "email3@example.com",
                    Name = "name3",
                    RegistrationDate = new DateTime(2005, 6, 3, 9, 12, 11)
                }
            };
    }
}