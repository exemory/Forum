using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using ExpectedObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Service.DataTransferObjects;
using Service.Exceptions;
using Service.Services;
using Xunit;

namespace Service.Tests
{
    public class PostServiceTests
    {
        private readonly PostService _sut;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();

        private readonly Mock<UserManager<User>> _userManagerMock =
            new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        public PostServiceTests()
        {
            _sut = new PostService(_unitOfWorkMock.Object, _userManagerMock.Object, UnitTestHelper.CreateMapper());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPost_WhenPostExists()
        {
            var post = PostListWithAuthors.First();
            _unitOfWorkMock.Setup(u => u.PostRepository.GetByIdWithDetailsAsync(post.Id))
                .ReturnsAsync(PostListWithAuthors.First());
            var expected = PostWithDetailsDtoList.First();

            var result = await _sut.GetByIdAsync(post.Id);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenPostDoesNotExist()
        {
            var nonexistentPostId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.PostRepository.GetByIdWithDetailsAsync(nonexistentPostId))
                .ReturnsAsync((Post) null);

            Func<Task> result = async () => await _sut.GetByIdAsync(nonexistentPostId);

            await result.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetByThreadAsync_ShouldReturnPosts_WhenThreadExists()
        {
            var thread = TestThread;
            var expected = PostWithDetailsDtoList.Where(p => p.ThreadId == thread.Id);

            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(thread.Id))
                .ReturnsAsync(TestThread);
            _unitOfWorkMock.Setup(u => u.PostRepository.GetThreadPostsWithDetailsAsync(thread.Id))
                .ReturnsAsync((Guid id) => PostListWithAuthors.Where(p => p.ThreadId == id));

            var result = await _sut.GetByThreadAsync(thread.Id);

            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Fact]
        public async Task GetByThreadAsync_ShouldFail_WhenThreadDoesNotExist()
        {
            var nonexistentThreadId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(nonexistentThreadId))
                .ReturnsAsync((Thread) null);

            Func<Task> result = async () => await _sut.GetByThreadAsync(nonexistentThreadId);

            await result.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreatePost()
        {
            var postDto = PostCreationDto;
            var author = TestUser;
            var expectedToAdd = CreatedPostToBeSaved
                .ToExpectedObject(o => o.Ignore(t => t.Author.ConcurrencyStamp));
            var expected = CreatedPostDto;

            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(postDto.ThreadId))
                .ReturnsAsync(TestThread);
            _userManagerMock.Setup(um => um.FindByIdAsync(author.Id.ToString()))
                .ReturnsAsync(TestUser);
            _unitOfWorkMock.Setup(u => u.PostRepository.Add(It.IsAny<Post>()));

            var result = await _sut.CreateAsync(postDto, author.Id);

            result.Should().BeEquivalentTo(expected);

            _unitOfWorkMock.Verify(uow =>
                uow.PostRepository.Add(It.Is<Post>(p => expectedToAdd.Equals(p))), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenThreadDoesNotExist()
        {
            var postDto = PostCreationDto;
            var authorId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(postDto.ThreadId))
                .ReturnsAsync((Thread) null);

            Func<Task> result = async () => await _sut.CreateAsync(postDto, authorId);

            await result.Should().ThrowAsync<NotFoundException>();

            _unitOfWorkMock.Verify(uow => uow.PostRepository.Add(It.IsAny<Post>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenAuthorDoesNotExist()
        {
            var postDto = PostCreationDto;
            var nonexistentAuthorId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(postDto.ThreadId))
                .ReturnsAsync(TestThread);
            _userManagerMock.Setup(um => um.FindByIdAsync(nonexistentAuthorId.ToString()))
                .ReturnsAsync((User) null);

            Func<Task> result = async () => await _sut.CreateAsync(postDto, nonexistentAuthorId);

            await result.Should().ThrowAsync<ForumException>()
                .WithMessage($"User with id '{nonexistentAuthorId}' does not exist");

            _unitOfWorkMock.Verify(uow => uow.PostRepository.Add(It.IsAny<Post>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenThreadIsClosed()
        {
            var postDto = PostCreationDto;
            var author = TestUser;
            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(postDto.ThreadId))
                .ReturnsAsync(ClosedThread);
            _userManagerMock.Setup(um => um.FindByIdAsync(author.Id.ToString()))
                .ReturnsAsync(TestUser);

            Func<Task> result = async () => await _sut.CreateAsync(postDto, author.Id);

            await result.Should().ThrowAsync<ForumException>()
                .WithMessage("Thread is closed for posting");

            _unitOfWorkMock.Verify(uow => uow.PostRepository.Add(It.IsAny<Post>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePost_WhenPostExists()
        {
            var post = TestPost;
            var postDto = PostUpdateDto;
            var expoctedToUpdate = UpdatedPostToBeSaved.ToExpectedObject();

            _unitOfWorkMock.Setup(u => u.PostRepository.GetByIdAsync(post.Id))
                .ReturnsAsync(TestPost);

            await _sut.UpdateAsync(post.Id, postDto);

            _unitOfWorkMock.Verify(uow =>
                uow.PostRepository.Update(It.Is<Post>(p => expoctedToUpdate.Equals(p))), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenPostDoesNotExist()
        {
            var nonexistentPostId = Guid.NewGuid();
            var postDto = PostUpdateDto;
            _unitOfWorkMock.Setup(u => u.PostRepository.GetByIdAsync(nonexistentPostId))
                .ReturnsAsync((Post) null);

            Func<Task> result = async () => await _sut.UpdateAsync(nonexistentPostId, postDto);

            await result.Should().ThrowAsync<NotFoundException>();

            _unitOfWorkMock.Verify(uow => uow.PostRepository.Update(It.IsAny<Post>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeletePost_WhenPostExists()
        {
            var post = TestPost;
            var expectedToDelete = TestPost.ToExpectedObject();

            _unitOfWorkMock.Setup(u => u.PostRepository.GetByIdAsync(post.Id))
                .ReturnsAsync(TestPost);

            await _sut.DeleteAsync(post.Id);

            _unitOfWorkMock.Verify(uow =>
                uow.PostRepository.Delete(It.Is<Post>(p => expectedToDelete.Equals(p))), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenPostDoesNotExist()
        {
            var nonexistentPostId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.PostRepository.GetByIdAsync(nonexistentPostId))
                .ReturnsAsync((Post) null);

            Func<Task> result = async () => await _sut.DeleteAsync(nonexistentPostId);

            await result.Should().ThrowAsync<NotFoundException>();

            _unitOfWorkMock.Verify(uow => uow.PostRepository.Delete(It.IsAny<Post>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        private static IEnumerable<Post> PostListWithAuthors =>
            new List<Post>
            {
                new Post
                {
                    Id = new Guid("d4376327-f24d-423e-9226-8f85117fe117"),
                    Content = "Vivamus ante sem, vehicula at euismod eu, luctus eleifend erat.",
                    ThreadId = new Guid("8fa32919-5dd4-4e88-90e8-98c9323bdbb6"),
                    AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    PublishDate = new DateTime(2021, 8, 14, 19, 25, 31),
                    Author = new User
                    {
                        Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                        UserName = "test1",
                        Email = "test1@ukr.net",
                        Name = "test1",
                        RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
                    }
                },
                new Post
                {
                    Id = new Guid("0db39598-3cf9-4e98-9d21-50a2c55cf5a1"),
                    Content = "Nulla eget",
                    ThreadId = new Guid("708fb0d0-94c7-4460-95bf-22f18ee38f29"),
                    AuthorId = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    PublishDate = new DateTime(2021, 8, 15, 13, 5, 53),
                    Author = new User
                    {
                        Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                        UserName = "test2",
                        Email = "test2@example.com",
                        Name = "test2",
                        RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
                    }
                },
                new Post
                {
                    Id = new Guid("074b6e15-965b-4a06-add1-302014c4e589"),
                    Content = "Aenean blandit luctus convallis",
                    ThreadId = new Guid("8fa32919-5dd4-4e88-90e8-98c9323bdbb6"),
                    AuthorId = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                    PublishDate = new DateTime(2021, 8, 17, 18, 18, 44),
                    Author = new User
                    {
                        Id = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                        UserName = "test3",
                        Email = "test3@example.com",
                        Name = "test3",
                        RegistrationDate = new DateTime(2005, 6, 3, 9, 12, 11)
                    }
                }
            };

        private static IEnumerable<PostWithDetailsDto> PostWithDetailsDtoList =>
            new List<PostWithDetailsDto>
            {
                new PostWithDetailsDto
                {
                    Id = new Guid("d4376327-f24d-423e-9226-8f85117fe117"),
                    Content = "Vivamus ante sem, vehicula at euismod eu, luctus eleifend erat.",
                    ThreadId = new Guid("8fa32919-5dd4-4e88-90e8-98c9323bdbb6"),
                    PublishDate = new DateTime(2021, 8, 14, 19, 25, 31),
                    Author = new UserDto
                    {
                        Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                        Username = "test1",
                        Name = "test1",
                        RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
                    }
                },
                new PostWithDetailsDto
                {
                    Id = new Guid("0db39598-3cf9-4e98-9d21-50a2c55cf5a1"),
                    Content = "Nulla eget",
                    ThreadId = new Guid("708fb0d0-94c7-4460-95bf-22f18ee38f29"),
                    PublishDate = new DateTime(2021, 8, 15, 13, 5, 53),
                    Author = new UserDto
                    {
                        Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                        Username = "test2",
                        Name = "test2",
                        RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
                    }
                },
                new PostWithDetailsDto
                {
                    Id = new Guid("074b6e15-965b-4a06-add1-302014c4e589"),
                    Content = "Aenean blandit luctus convallis",
                    ThreadId = new Guid("8fa32919-5dd4-4e88-90e8-98c9323bdbb6"),
                    PublishDate = new DateTime(2021, 8, 17, 18, 18, 44),
                    Author = new UserDto
                    {
                        Id = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                        Username = "test3",
                        Name = "test3",
                        RegistrationDate = new DateTime(2005, 6, 3, 9, 12, 11)
                    }
                }
            };

        private static User TestUser => new User
        {
            Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
            UserName = "test1",
            Email = "test1@ukr.net",
            Name = "test1",
            RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
        };

        private static Post TestPost =>
            new Post
            {
                Id = new Guid("d4376327-f24d-423e-9226-8f85117fe117"),
                Content = "Vivamus ante sem, vehicula at euismod eu, luctus eleifend erat.",
                ThreadId = new Guid("8fa32919-5dd4-4e88-90e8-98c9323bdbb6"),
                AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                PublishDate = new DateTime(2021, 8, 14, 19, 25, 31)
            };

        private static Thread TestThread => new Thread
        {
            Id = new Guid("8fa32919-5dd4-4e88-90e8-98c9323bdbb6"),
            Topic = "Test thread topic",
            Closed = false,
            CreationDate = new DateTime(2017, 4, 10, 16, 27, 25),
            AuthorId = new Guid("6fbd2ad6-e32e-46fd-80f2-59125afb9700")
        };

        private static Thread ClosedThread => new Thread
        {
            Id = new Guid("8fa32919-5dd4-4e88-90e8-98c9323bdbb6"),
            Topic = "Closed thread topic",
            Closed = true,
            CreationDate = new DateTime(2015, 11, 22, 11, 54, 18),
            AuthorId = new Guid("6fbd2ad6-e32e-46fd-80f2-59125afb9700")
        };

        private static PostCreationDto PostCreationDto => new PostCreationDto()
        {
            Content = "New post content",
            ThreadId = new Guid("8fa32919-5dd4-4e88-90e8-98c9323bdbb6")
        };

        private static Post CreatedPostToBeSaved =>
            new Post
            {
                Content = "New post content",
                ThreadId = new Guid("8fa32919-5dd4-4e88-90e8-98c9323bdbb6"),
                Author = new User
                {
                    Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    UserName = "test1",
                    Email = "test1@ukr.net",
                    Name = "test1",
                    RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
                }
            };

        private static PostWithDetailsDto CreatedPostDto =>
            new PostWithDetailsDto
            {
                Content = "New post content",
                ThreadId = new Guid("8fa32919-5dd4-4e88-90e8-98c9323bdbb6"),
                Author = new UserDto
                {
                    Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    Username = "test1",
                    Name = "test1",
                    RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
                }
            };

        private static PostUpdateDto PostUpdateDto => new PostUpdateDto()
        {
            Content = "Updated post content"
        };

        private static Post UpdatedPostToBeSaved =>
            new Post
            {
                Id = new Guid("d4376327-f24d-423e-9226-8f85117fe117"),
                Content = "Updated post content",
                ThreadId = new Guid("8fa32919-5dd4-4e88-90e8-98c9323bdbb6"),
                AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                PublishDate = new DateTime(2021, 8, 14, 19, 25, 31)
            };
    }
}