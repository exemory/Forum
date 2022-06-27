using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Data.Repositories;
using FluentAssertions;
using Xunit;

namespace Data.Tests
{
    public class ThreadRepositoryTests
    {
        private readonly ThreadRepository _sut;
        private readonly ForumContext _context = new ForumContext(UnitTestHelper.GetUnitTestDbOptions());

        public ThreadRepositoryTests()
        {
            _sut = new ThreadRepository(_context);
        }

        [Theory]
        [MemberData(nameof(ThreadIds_TestData))]
        public async Task GetByIdWithDetailsAsync_ShouldReturnThread_WhenThreadExists(Guid threadId)
        {
            var expected = ThreadsWithIncludedAuthors.First(t => t.Id == threadId);

            var result = await _sut.GetByIdWithDetailsAsync(threadId);

            result.Should().BeEquivalentTo(expected, o =>
                o.Excluding(u => u.Author.Threads)
                    .Excluding(u => u.Author.ConcurrencyStamp));
        }

        [Fact]
        public async Task GetByIdWithDetailsAsync_ShouldReturnNull_WhenThreadDoesNotExist()
        {
            var nonexistentThreadId = Guid.NewGuid();

            var result = await _sut.GetByIdWithDetailsAsync(nonexistentThreadId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllWithDetailsAsync_ShouldReturnAllThreads()
        {
            var expected = ThreadsWithIncludedAuthors;

            var result = await _sut.GetAllWithDetailsAsync();

            result.Should().BeEquivalentTo(expected, o =>
                    o.Excluding(t => t.Author.Threads)
                        .Excluding(t => t.Author.ConcurrencyStamp))
                .And
                .BeInDescendingOrder(t => t.CreationDate);
        }

        public static IEnumerable<object[]> ThreadIds_TestData()
        {
            yield return new object[] {new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3")};
            yield return new object[] {new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85")};
            yield return new object[] {new Guid("5891e6dc-09ec-4883-9040-80c38c0318ab")};
        }

        private static IEnumerable<Thread> ThreadsWithIncludedAuthors =>
            new List<Thread>
            {
                new Thread
                {
                    Id = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    Topic = "Thread topic 1",
                    Closed = false,
                    CreationDate = new DateTime(2012, 12, 8, 5, 16, 30),
                    AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    Author = new User
                    {
                        Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                        UserName = "username1",
                        Email = "email1@example.com",
                        Name = "name1",
                        RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
                    }
                },
                new Thread
                {
                    Id = new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85"),
                    Topic = "Thread topic 2",
                    Closed = true,
                    CreationDate = new DateTime(2017, 3, 3, 5, 12, 51),
                    AuthorId = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    Author = new User
                    {
                        Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                        UserName = "username2",
                        Email = "email2@example.com",
                        Name = "name2",
                        RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
                    }
                },
                new Thread
                {
                    Id = new Guid("5891e6dc-09ec-4883-9040-80c38c0318ab"),
                    Topic = "Thread topic 3",
                    Closed = false,
                    CreationDate = new DateTime(2016, 9, 24, 9, 18, 11),
                    AuthorId = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    Author = new User
                    {
                        Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                        UserName = "username2",
                        Email = "email2@example.com",
                        Name = "name2",
                        RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
                    }
                }
            };
    }
}