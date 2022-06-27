using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Data.Tests
{
    public class RepositoryTests
    {
        private readonly Repository<Post> _sut;
        private readonly ForumContext _context = new ForumContext(UnitTestHelper.GetUnitTestDbOptions());

        public RepositoryTests()
        {
            _sut = new Repository<Post>(_context);
        }
        
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            var expected = PostList;

            var result = await _sut.GetAllAsync();

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(PostIds_TestData))]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists(Guid postId)
        {
            var expected = PostList.First(p => p.Id == postId);

            var result = await _sut.GetByIdAsync(postId);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            var nonexistentPostId = Guid.NewGuid();

            var result = await _sut.GetByIdAsync(nonexistentPostId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Add_ShouldAddEntity()
        {
            var post = PostToAdd;

            _sut.Add(post);
            await _context.SaveChangesAsync();

            post.Id.Should().NotBeEmpty();
            post.Should().BeEquivalentTo(PostToAdd, o => o.Excluding(p => p.Id));

            var addedPostInDb = await _context.Posts.FirstAsync(p => p.Id == post.Id);

            addedPostInDb.Should().BeEquivalentTo(PostToAdd, o => o.Excluding(p => p.Id));
        }

        [Fact]
        public async Task Update_ShouldUpdateEntity()
        {
            var post = PostToUpdate;

            _sut.Update(post);
            await _context.SaveChangesAsync();

            post.Should().BeEquivalentTo(PostToUpdate);

            var updatedPostInDb = await _context.Posts.FirstAsync(p => p.Id == post.Id);

            updatedPostInDb.Should().BeEquivalentTo(PostToUpdate);
        }

        [Theory]
        [MemberData(nameof(PostIds_TestData))]
        public async Task Delete_ShouldDeleteEntity(Guid postId)
        {
            var post = PostList.First(p => p.Id == postId);
            var expected = PostList.Where(p => p.Id != post.Id);

            _sut.Delete(post);
            await _context.SaveChangesAsync();

            var postsInDb = await _context.Posts.ToListAsync();

            postsInDb.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(PostIds_TestData))]
        public async Task DeleteById_ShouldDeleteEntity(Guid postId)
        {
            var expected = PostList.Where(p => p.Id != postId);

            _sut.DeleteById(postId);
            await _context.SaveChangesAsync();

            var postsInDb = await _context.Posts.ToListAsync();

            postsInDb.Should().BeEquivalentTo(expected);
        }
        
        public static IEnumerable<object[]> PostIds_TestData()
        {
            yield return new object[] {new Guid("d4376327-f24d-423e-9226-8f85117fe117")};
            yield return new object[] {new Guid("7e845814-1b72-45ca-852e-01311adab752")};
            yield return new object[] {new Guid("61b61787-3488-48c1-bf3c-e76b1731f77f")};
        }

        private static IEnumerable<Post> PostList =>
            new List<Post>
            {
                new Post
                {
                    Id = new Guid("d4376327-f24d-423e-9226-8f85117fe117"),
                    Content = "Post content 1",
                    ThreadId = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    AuthorId = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                    PublishDate = new DateTime(2013, 1, 5, 19, 25, 31)
                },
                new Post
                {
                    Id = new Guid("0db39598-3cf9-4e98-9d21-50a2c55cf5a1"),
                    Content = "Post content 2",
                    ThreadId = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    PublishDate = new DateTime(2012, 12, 10, 13, 5, 53)
                },
                new Post
                {
                    Id = new Guid("074b6e15-965b-4a06-add1-302014c4e589"),
                    Content = "Post content 3",
                    ThreadId = new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85"),
                    AuthorId = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    PublishDate = new DateTime(2017, 3, 4, 9, 46, 9)
                },
                new Post
                {
                    Id = new Guid("7e845814-1b72-45ca-852e-01311adab752"),
                    Content = "Post content 4",
                    ThreadId = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    AuthorId = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    PublishDate = new DateTime(2013, 1, 8, 2, 20, 3)
                },
                new Post
                {
                    Id = new Guid("61b61787-3488-48c1-bf3c-e76b1731f77f"),
                    Content = "Post content 5",
                    ThreadId = new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85"),
                    AuthorId = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                    PublishDate = new DateTime(2017, 3, 3, 5, 18, 44)
                }
            };

        private static Post PostToAdd =>
            new Post
            {
                Content = "Test post content",
                AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                ThreadId = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3")
            };

        private static Post PostToUpdate =>
            new Post
            {
                Id = new Guid("d4376327-f24d-423e-9226-8f85117fe117"),
                Content = "Updated post content",
                AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                ThreadId = new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85")
            };
    }
}