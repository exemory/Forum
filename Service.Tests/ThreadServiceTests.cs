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
    public class ThreadServiceTests
    {
        private readonly ThreadService _sut;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();

        private readonly Mock<UserManager<User>> _userManagerMock =
            new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        public ThreadServiceTests()
        {
            _sut = new ThreadService(_unitOfWorkMock.Object, _userManagerMock.Object, UnitTestHelper.CreateMapper());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnThread_WhenThreadExists()
        {
            var thread = ThreadListWithAuthors.First();
            var expected = ThreadWithDetailsDtoList.First();

            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdWithDetailsAsync(thread.Id))
                .ReturnsAsync(ThreadListWithAuthors.First());

            var result = await _sut.GetByIdAsync(thread.Id);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenThreadDoesNotExist()
        {
            var nonexistentThreadId = Guid.NewGuid();

            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdWithDetailsAsync(nonexistentThreadId))
                .ReturnsAsync((Thread) null);

            Func<Task> result = async () => await _sut.GetByIdAsync(nonexistentThreadId);

            await result.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnThreads()
        {
            var expected = ThreadWithDetailsDtoList;

            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(ThreadListWithAuthors);

            var result = await _sut.GetAllAsync();

            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateThread_WhenAuthorExists()
        {
            var threadDto = ThreadCreationDto;
            var author = TestUser;
            var expectedToAdd = CreatedThreadToBeSaved
                .ToExpectedObject(o => o.Ignore(t => t.Author.ConcurrencyStamp));
            var expected = CreatedThreadDto;

            _userManagerMock.Setup(um => um.FindByIdAsync(author.Id.ToString()))
                .ReturnsAsync(TestUser);
            _unitOfWorkMock.Setup(u => u.ThreadRepository.Add(It.IsAny<Thread>()));

            var result = await _sut.CreateAsync(threadDto, author.Id);

            result.Should().BeEquivalentTo(expected);

            _unitOfWorkMock.Verify(uow =>
                uow.ThreadRepository.Add(It.Is<Thread>(t => expectedToAdd.Equals(t))), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenAuthorDoesNotExist()
        {
            var threadDto = ThreadCreationDto;
            var nonexistentAuthorId = Guid.NewGuid();

            _userManagerMock.Setup(um => um.FindByIdAsync(nonexistentAuthorId.ToString()))
                .ReturnsAsync((User) null);

            Func<Task> result = async () => await _sut.CreateAsync(threadDto, nonexistentAuthorId);

            await result.Should().ThrowAsync<ForumException>();

            _unitOfWorkMock.Verify(uow => uow.ThreadRepository.Add(It.IsAny<Thread>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateThread_WhenThreadExists()
        {
            var thread = TestThread;
            var threadDto = ThreadUpdateDto;
            var expectedToUpdate = UpdatedThreadToBeSaved.ToExpectedObject();

            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(thread.Id))
                .ReturnsAsync(TestThread);

            await _sut.UpdateAsync(thread.Id, threadDto);

            _unitOfWorkMock.Verify(uow =>
                uow.ThreadRepository.Update(It.Is<Thread>(t => expectedToUpdate.Equals(t))), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenThreadDoesNotExist()
        {
            var nonexistentThreadId = Guid.NewGuid();
            var threadDto = ThreadUpdateDto;

            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(nonexistentThreadId))
                .ReturnsAsync((Thread) null);

            Func<Task> result = async () => await _sut.UpdateAsync(nonexistentThreadId, threadDto);

            await result.Should().ThrowAsync<NotFoundException>();

            _unitOfWorkMock.Verify(uow => uow.ThreadRepository.Update(It.IsAny<Thread>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteThread_WhenThreadExists()
        {
            var thread = TestThread;
            var expectedToDelete = TestThread.ToExpectedObject();

            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(thread.Id))
                .ReturnsAsync(TestThread);

            await _sut.DeleteAsync(thread.Id);

            _unitOfWorkMock.Verify(uow =>
                uow.ThreadRepository.Delete(It.Is<Thread>(t => expectedToDelete.Equals(t))), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenThreadDoesNotExist()
        {
            var nonexistentThreadId = Guid.NewGuid();

            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(nonexistentThreadId))
                .ReturnsAsync((Thread) null);

            Func<Task> result = async () => await _sut.DeleteAsync(nonexistentThreadId);

            await result.Should().ThrowAsync<NotFoundException>();

            _unitOfWorkMock.Verify(uow => uow.ThreadRepository.Delete(It.IsAny<Thread>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateThreadStatus_WhenThreadExists()
        {
            var thread = TestThread;
            var statusDto = ThreadStatusUpdateDto;
            var expectedToUpdate = ThreadWithUpdatedStatusToBeSaved.ToExpectedObject();

            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(thread.Id))
                .ReturnsAsync(TestThread);

            await _sut.UpdateStatusAsync(thread.Id, statusDto);

            _unitOfWorkMock.Verify(uow =>
                uow.ThreadRepository.Update(It.Is<Thread>(t => expectedToUpdate.Equals(t))), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldFail_WhenThreadDoesNotExist()
        {
            var nonExistentThreadId = Guid.NewGuid();
            var statusDto = ThreadStatusUpdateDto;

            _unitOfWorkMock.Setup(u => u.ThreadRepository.GetByIdAsync(nonExistentThreadId))
                .ReturnsAsync((Thread) null);

            Func<Task> result = async () => await _sut.UpdateStatusAsync(nonExistentThreadId, statusDto);

            await result.Should().ThrowAsync<NotFoundException>();

            _unitOfWorkMock.Verify(uow => uow.ThreadRepository.Update(It.IsAny<Thread>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        private static IEnumerable<Thread> ThreadListWithAuthors =>
            new List<Thread>
            {
                new Thread
                {
                    Id = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    Topic = "Thread topic 1",
                    Closed = false,
                    CreationDate = new DateTime(2020, 5, 8, 5, 16, 30),
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
                    Closed = false,
                    CreationDate = new DateTime(2020, 5, 3, 5, 12, 51),
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
                    CreationDate = new DateTime(2020, 2, 24, 9, 18, 11),
                    AuthorId = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                    Author = new User
                    {
                        Id = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                        UserName = "username3",
                        Email = "email3@example.com",
                        Name = "name3",
                        RegistrationDate = new DateTime(2005, 6, 3, 9, 12, 11)
                    }
                },
            };

        private static IEnumerable<ThreadWithDetailsDto> ThreadWithDetailsDtoList =>
            new List<ThreadWithDetailsDto>
            {
                new ThreadWithDetailsDto
                {
                    Id = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    Topic = "Thread topic 1",
                    Closed = false,
                    CreationDate = new DateTime(2020, 5, 8, 5, 16, 30),
                    Author = new UserDto
                    {
                        Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                        Username = "username1",
                        Name = "name1",
                        RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
                    }
                },
                new ThreadWithDetailsDto
                {
                    Id = new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85"),
                    Topic = "Thread topic 2",
                    Closed = false,
                    CreationDate = new DateTime(2020, 5, 3, 5, 12, 51),
                    Author = new UserDto
                    {
                        Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                        Username = "username2",
                        Name = "name2",
                        RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
                    }
                },
                new ThreadWithDetailsDto
                {
                    Id = new Guid("5891e6dc-09ec-4883-9040-80c38c0318ab"),
                    Topic = "Thread topic 3",
                    Closed = false,
                    CreationDate = new DateTime(2020, 2, 24, 9, 18, 11),
                    Author = new UserDto
                    {
                        Id = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                        Username = "username3",
                        Name = "name3",
                        RegistrationDate = new DateTime(2005, 6, 3, 9, 12, 11)
                    }
                },
            };

        private static User TestUser => new User
        {
            Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
            UserName = "test_username",
            Email = "test_email@example.com",
            Name = "test_name",
            RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
        };

        private static ThreadCreationDto ThreadCreationDto =>
            new ThreadCreationDto
            {
                Topic = "New thread topic"
            };

        private static Thread CreatedThreadToBeSaved =>
            new Thread
            {
                Topic = "New thread topic",
                Author = new User
                {
                    Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    UserName = "test_username",
                    Email = "test_email@example.com",
                    Name = "test_name",
                    RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
                }
            };

        private static ThreadWithDetailsDto CreatedThreadDto =>
            new ThreadWithDetailsDto
            {
                Topic = "New thread topic",
                Author = new UserDto
                {
                    Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    Username = "test_username",
                    Name = "test_name",
                    RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
                }
            };

        private static Thread TestThread =>
            new Thread
            {
                Id = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                Topic = "Test thread topic",
                Closed = false,
                CreationDate = new DateTime(2020, 5, 8, 5, 16, 30),
                AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2")
            };

        private static ThreadUpdateDto ThreadUpdateDto =>
            new ThreadUpdateDto
            {
                Topic = "Updated thread topic"
            };

        private static Thread UpdatedThreadToBeSaved =>
            new Thread
            {
                Id = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                Topic = "Updated thread topic",
                Closed = false,
                CreationDate = new DateTime(2020, 5, 8, 5, 16, 30),
                AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2")
            };

        private static ThreadStatusUpdateDto ThreadStatusUpdateDto =>
            new ThreadStatusUpdateDto
            {
                Closed = true
            };

        private static Thread ThreadWithUpdatedStatusToBeSaved =>
            new Thread
            {
                Id = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                Topic = "Test thread topic",
                Closed = true,
                CreationDate = new DateTime(2020, 5, 8, 5, 16, 30),
                AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2")
            };
    }
}