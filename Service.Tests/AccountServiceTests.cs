using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
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
    public class AccountServiceTests
    {
        private readonly AccountService _sut;

        private readonly Mock<UserManager<User>> _userManagerMock =
            new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        public AccountServiceTests()
        {
            _sut = new AccountService(_userManagerMock.Object, UnitTestHelper.CreateMapper());
        }

        [Fact]
        public async Task GetInfoAsync_ShouldReturnAccountInfo_WhenUserExists()
        {
            var user = TestUser;
            var expectedToCheck = TestUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(TestUser);
            _userManagerMock.Setup(um => um.GetRolesAsync(It.Is<User>(u => expectedToCheck.Equals(u))))
                .ReturnsAsync(TestUserRoles.ToList());

            var result = await _sut.GetInfoAsync(user.Id);

            result.Should().BeEquivalentTo(TestUserWithDetailsDto);
        }

        [Fact]
        public async Task GetInfoAsync_ShouldFail_WhenUserDoesNotExist()
        {
            var nonexistentUserId = Guid.NewGuid();

            _userManagerMock.Setup(um => um.FindByIdAsync(nonexistentUserId.ToString()))
                .ReturnsAsync((User) null);

            Func<Task> result = async () => await _sut.GetInfoAsync(nonexistentUserId);

            await result.Should().ThrowAsync<ForumException>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            var user = TestUser;
            var accountDto = TestAccountUpdateDto;
            var expectedToCheck = TestUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));
            var expectedToUpdate = UpdatedUserToBeSaved
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(TestUser);
            _userManagerMock.Setup(um =>
                    um.CheckPasswordAsync(It.Is<User>(u => expectedToCheck.Equals(u)), accountDto.CurrentPassword))
                .ReturnsAsync(true);
            _userManagerMock.Setup(um => um.UpdateAsync(It.Is<User>(u => expectedToUpdate.Equals(u))))
                .ReturnsAsync(IdentityResult.Success);

            await _sut.UpdateAsync(user.Id, TestAccountUpdateDto);

            _userManagerMock.Verify(um =>
                    um.CheckPasswordAsync(It.Is<User>(u => expectedToUpdate.Equals(u)), accountDto.CurrentPassword),
                Times.Once);
            _userManagerMock.Verify(um =>
                um.UpdateAsync(It.Is<User>(u => expectedToUpdate.Equals(u))), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenUserDoesNotExist()
        {
            var nonexistentUserId = Guid.NewGuid();

            _userManagerMock.Setup(um => um.FindByIdAsync(nonexistentUserId.ToString()))
                .ReturnsAsync((User) null);

            Func<Task> result = async () => await _sut.UpdateAsync(nonexistentUserId, TestAccountUpdateDto);

            await result.Should().ThrowAsync<ForumException>()
                .WithMessage($"User with id {nonexistentUserId} does not exist");

            _userManagerMock.Verify(um =>
                um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _userManagerMock.Verify(um =>
                um.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenCurrentPasswordIsInvalid()
        {
            var user = TestUser;
            var accountDto = TestAccountUpdateDto;
            var expectedToCheck = TestUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(TestUser);
            _userManagerMock.Setup(um =>
                    um.CheckPasswordAsync(It.Is<User>(u => expectedToCheck.Equals(u)), accountDto.CurrentPassword))
                .ReturnsAsync(false);

            Func<Task> result = async () => await _sut.UpdateAsync(user.Id, TestAccountUpdateDto);

            await result.Should().ThrowAsync<ForumException>()
                .WithMessage($"Incorrect password");

            _userManagerMock.Verify(um =>
                    um.CheckPasswordAsync(It.Is<User>(u => expectedToCheck.Equals(u)), accountDto.CurrentPassword),
                Times.Once);
            _userManagerMock.Verify(um =>
                um.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenUserUpdateFailed()
        {
            var user = TestUser;
            var accountDto = TestAccountUpdateDto;
            var expectedToCheck = TestUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));
            var expectedToUpdate = UpdatedUserToBeSaved
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(TestUser);
            _userManagerMock.Setup(um =>
                    um.CheckPasswordAsync(It.Is<User>(u => expectedToCheck.Equals(u)), accountDto.CurrentPassword))
                .ReturnsAsync(true);
            _userManagerMock.Setup(um => um.UpdateAsync(It.Is<User>(u => expectedToUpdate.Equals(u))))
                .ReturnsAsync(IdentityError);

            Func<Task> result = async () => await _sut.UpdateAsync(user.Id, TestAccountUpdateDto);

            await result.Should().ThrowAsync<ForumException>()
                .WithMessage(ExpectedErrorMessage);

            _userManagerMock.Verify(um =>
                    um.CheckPasswordAsync(It.Is<User>(u => expectedToUpdate.Equals(u)), accountDto.CurrentPassword),
                Times.Once);
            _userManagerMock.Verify(um =>
                um.UpdateAsync(It.Is<User>(u => expectedToUpdate.Equals(u))), Times.Once);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ShouldUpdateUserPassword()
        {
            var user = TestUser;
            var passwordDto = TestPasswordChangeDto;
            var expectedUserToChangePassword = TestUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(TestUser);
            _userManagerMock.Setup(um =>
                    um.ChangePasswordAsync(It.Is<User>(u => expectedUserToChangePassword.Equals(u)),
                        passwordDto.CurrentPassword, passwordDto.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            await _sut.ChangePasswordAsync(user.Id, TestPasswordChangeDto);

            _userManagerMock.Verify(um =>
                um.ChangePasswordAsync(It.Is<User>(u => expectedUserToChangePassword.Equals(u)),
                    passwordDto.CurrentPassword, passwordDto.NewPassword), Times.Once);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ShouldFail_WhenUserDoesNotExist()
        {
            var nonexistentUserId = Guid.NewGuid();

            _userManagerMock.Setup(um => um.FindByIdAsync(nonexistentUserId.ToString()))
                .ReturnsAsync((User) null);

            Func<Task> result = async () => await _sut.ChangePasswordAsync(nonexistentUserId, TestPasswordChangeDto);

            await result.Should().ThrowAsync<ForumException>()
                .WithMessage($"User with id {nonexistentUserId} does not exist");

            _userManagerMock.Verify(um =>
                um.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ShouldFail_WhenChangePasswordFails()
        {
            var user = TestUser;
            var passwordDto = TestPasswordChangeDto;
            var expectedUserToChangePassword = TestUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(TestUser);
            _userManagerMock.Setup(um =>
                    um.ChangePasswordAsync(It.Is<User>(u => expectedUserToChangePassword.Equals(u)),
                        passwordDto.CurrentPassword, passwordDto.NewPassword))
                .ReturnsAsync(IdentityError);

            Func<Task> result = async () => await _sut.ChangePasswordAsync(user.Id, TestPasswordChangeDto);

            await result.Should().ThrowAsync<ForumException>()
                .WithMessage(ExpectedErrorMessage);

            _userManagerMock.Verify(um =>
                um.ChangePasswordAsync(It.Is<User>(u => expectedUserToChangePassword.Equals(u)),
                    passwordDto.CurrentPassword, passwordDto.NewPassword), Times.Once);
        }

        private static IdentityResult IdentityError => 
            IdentityResult.Failed(new IdentityError
            {
                Code = "TestError", 
                Description = "Test error description"
            });

        private static string ExpectedErrorMessage => "Test error description";
        
        private static User TestUser => new User
        {
            Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
            UserName = "test_username",
            Email = "test_email@example.com",
            Name = "test_name",
            RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
        };

        private static IEnumerable<string> TestUserRoles => new List<string> {"User"};

        private static UserWithDetailsDto TestUserWithDetailsDto => new UserWithDetailsDto
        {
            Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
            Username = "test_username",
            Email = "test_email@example.com",
            Name = "test_name",
            RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59),
            Roles = TestUserRoles
        };

        private static AccountUpdateDto TestAccountUpdateDto => new AccountUpdateDto
        {
            Username = "new_username",
            Email = "new_email@example.com",
            Name = "new_name",
            CurrentPassword = "test_password"
        };

        private static User UpdatedUserToBeSaved => new User
        {
            Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
            UserName = "new_username",
            Email = "new_email@example.com",
            Name = "new_name",
            RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
        };

        private static PasswordChangeDto TestPasswordChangeDto => new PasswordChangeDto
        {
            CurrentPassword = "test_password",
            NewPassword = "new_password"
        };
    }
}