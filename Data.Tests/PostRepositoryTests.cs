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
    public class PostRepositoryTests
    {
        private readonly PostRepository _sut;
        private readonly ForumContext _context = new ForumContext(UnitTestHelper.GetUnitTestDbOptions());

        public PostRepositoryTests()
        {
            _sut = new PostRepository(_context);
        }

        [Theory]
        [MemberData(nameof(PostIds_TestData))]
        public async Task GetByIdWithDetailsAsync_ShouldReturnPost_WhenPostExists(Guid postId)
        {
            var expected = PostsWithIncludedAuthors.First(p => p.Id == postId);

            var result = await _sut.GetByIdWithDetailsAsync(postId);

            result.Should().BeEquivalentTo(expected, o =>
                o.Excluding(p => p.Author.Posts)
                    .Excluding(p => p.Author.ConcurrencyStamp));
        }

        [Fact]
        public async Task GetByIdWithDetailsAsync_ShouldReturnNull_WhenPostDoesNotExist()
        {
            var nonexistentPostId = Guid.NewGuid();

            var result = await _sut.GetByIdWithDetailsAsync(nonexistentPostId);

            result.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(ThreadIds_TestData))]
        public async Task GetThreadPostsWithDetailsAsync_ShouldReturnThreadPosts_WhenThreadExists(Guid threadId)
        {
            var expected = PostsWithIncludedAuthors.Where(p => p.ThreadId == threadId);

            var result = await _sut.GetByThreadIdWithDetailsAsync(threadId);

            result.Should().BeEquivalentTo(expected, o =>
                    o.Excluding(p => p.Author.Posts)
                        .Excluding(p => p.Author.ConcurrencyStamp))
                .And
                .BeInAscendingOrder(p => p.PublishDate);
        }
        
        [Fact]
        public async Task GetThreadPostsWithDetailsAsync_ShouldReturnEmptyCollection_WhenThreadDoesNotExist()
        {
            var nonexistentThreadId = Guid.NewGuid();

            var result = await _sut.GetByThreadIdWithDetailsAsync(nonexistentThreadId);

            result.Should().BeEmpty();
        }

        public static IEnumerable<object[]> PostIds_TestData()
        {
            yield return new object[] {new Guid("d4376327-f24d-423e-9226-8f85117fe117")};
            yield return new object[] {new Guid("7e845814-1b72-45ca-852e-01311adab752")};
            yield return new object[] {new Guid("61b61787-3488-48c1-bf3c-e76b1731f77f")};
        }

        public static IEnumerable<object[]> ThreadIds_TestData()
        {
            yield return new object[] {new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3")};
            yield return new object[] {new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85")};
            yield return new object[] {new Guid("5891e6dc-09ec-4883-9040-80c38c0318ab")};
        }

        private static IEnumerable<Post> PostsWithIncludedAuthors =>
            new List<Post>
            {
                new Post
                {
                    Id = new Guid("d4376327-f24d-423e-9226-8f85117fe117"),
                    Content = "Post content 1",
                    ThreadId = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    AuthorId = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                    PublishDate = new DateTime(2013, 1, 5, 19, 25, 31),
                    Author = new User
                    {
                        Id = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                        UserName = "username3",
                        Email = "email3@example.com",
                        Name = "name3",
                        RegistrationDate = new DateTime(2005, 6, 3, 9, 12, 11)
                    }
                },
                new Post
                {
                    Id = new Guid("0db39598-3cf9-4e98-9d21-50a2c55cf5a1"),
                    Content = "Post content 2",
                    ThreadId = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    PublishDate = new DateTime(2012, 12, 10, 13, 5, 53),
                    Author = new User
                    {
                        Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                        UserName = "username1",
                        Email = "email1@example.com",
                        Name = "name1",
                        RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
                    }
                },
                new Post
                {
                    Id = new Guid("074b6e15-965b-4a06-add1-302014c4e589"),
                    Content = "Post content 3",
                    ThreadId = new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85"),
                    AuthorId = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    PublishDate = new DateTime(2017, 3, 4, 9, 46, 9),
                    Author = new User
                    {
                        Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                        UserName = "username2",
                        Email = "email2@example.com",
                        Name = "name2",
                        RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
                    }
                },
                new Post
                {
                    Id = new Guid("7e845814-1b72-45ca-852e-01311adab752"),
                    Content = "Post content 4",
                    ThreadId = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    AuthorId = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    PublishDate = new DateTime(2013, 1, 8, 2, 20, 3),
                    Author = new User
                    {
                        Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                        UserName = "username2",
                        Email = "email2@example.com",
                        Name = "name2",
                        RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
                    }
                },
                new Post
                {
                    Id = new Guid("61b61787-3488-48c1-bf3c-e76b1731f77f"),
                    Content = "Post content 5",
                    ThreadId = new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85"),
                    AuthorId = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                    PublishDate = new DateTime(2017, 3, 3, 5, 18, 44),
                    Author = new User
                    {
                        Id = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                        UserName = "username3",
                        Email = "email3@example.com",
                        Name = "name3",
                        RegistrationDate = new DateTime(2005, 6, 3, 9, 12, 11)
                    }
                }
            };
    }
}