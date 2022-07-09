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
    public class UserServiceTests
    {
        private readonly UserService _sut;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();

        private readonly Mock<UserManager<User>> _userManagerMock =
            new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        public UserServiceTests()
        {
            _sut = new UserService(_unitOfWorkMock.Object, _userManagerMock.Object, UnitTestHelper.CreateMapper());
        }

        [Fact]
        public async Task GetInfoByUsernameAsync_ShouldReturnUser_WhenUserExists()
        {
            var user = RegularUser;
            var expectedUser = RegularUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));
            var expected = ExpectedUserProfileInfoDto;

            _userManagerMock.Setup(um => um.FindByNameAsync(user.UserName))
                .ReturnsAsync(RegularUser);
            _userManagerMock.Setup(um => um.GetRolesAsync(It.Is<User>(u => expectedUser.Equals(u))))
                .ReturnsAsync(GetUserRoles(user).ToList());

            var result = await _sut.GetInfoByUsernameAsync(user.UserName);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetInfoByUsernameAsync_ShouldFail_WhenUserDoesNotExist()
        {
            const string nonexistentUsername = "nonexistent_username";

            _userManagerMock.Setup(um => um.FindByNameAsync(nonexistentUsername))
                .ReturnsAsync((User) null);

            Func<Task> result = async () => await _sut.GetInfoByUsernameAsync(nonexistentUsername);

            await result.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnUsers()
        {
            var expected = UserWithDetailsDtoList;

            _unitOfWorkMock.Setup(u => u.UserRepository.GetAllAsync())
                .ReturnsAsync(UserList);
            _userManagerMock.Setup(u => u.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) => GetUserRoles(user).ToList());

            var result = await _sut.GetAllAsync();

            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Theory]
        [MemberData(nameof(UpdateRoleAsync_TestData))]
        public async Task UpdateRoleAsync_ShouldUpdateRole(User user, string role)
        {
            var roleDto = new UserRoleUpdateDto {Role = role};
            var userRoles = GetUserRoles(user);
            var expectedUser = user
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(It.Is<User>(u => expectedUser.Equals(u))))
                .ReturnsAsync(userRoles.ToList());

            await _sut.UpdateRoleAsync(user.Id, roleDto);

            _userManagerMock.Verify(um =>
                um.RemoveFromRolesAsync(It.Is<User>(u => expectedUser.Equals(u)),
                    It.Is<IEnumerable<string>>(r => userRoles.ToExpectedObject().Equals(r))), Times.Once);
            _userManagerMock.Verify(um =>
                um.AddToRoleAsync(It.Is<User>(u => expectedUser.Equals(u)), roleDto.Role), Times.Once);
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldFail_WhenUserDoesNotExist()
        {
            var nonexistentUserId = Guid.NewGuid();
            var roleDto = new UserRoleUpdateDto();

            _userManagerMock.Setup(um => um.FindByIdAsync(nonexistentUserId.ToString()))
                .ReturnsAsync((User) null);

            Func<Task> result = async () => await _sut.UpdateRoleAsync(nonexistentUserId, roleDto);

            await result.Should().ThrowAsync<NotFoundException>();

            _userManagerMock.Verify(um =>
                um.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            _userManagerMock.Verify(um =>
                um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldFail_WhenUserIsAdministrator()
        {
            var adminUser = AdminUser;
            var roleDto = new UserRoleUpdateDto();
            var expectedUser = AdminUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByIdAsync(adminUser.Id.ToString()))
                .ReturnsAsync(AdminUser);
            _userManagerMock.Setup(um => um.GetRolesAsync(It.Is<User>(u => expectedUser.Equals(u))))
                .ReturnsAsync(new List<string> {"Administrator"});

            Func<Task> result = async () => await _sut.UpdateRoleAsync(adminUser.Id, roleDto);

            await result.Should().ThrowAsync<ForumException>()
                .WithMessage("Impossible to change role of administrator");

            _userManagerMock.Verify(um =>
                um.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            _userManagerMock.Verify(um =>
                um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(UpdateRoleAsync_ShouldFail_TestData))]
        public async Task UpdateRoleAsync_ShouldFail_WhenUserIsAlreadyInThisRole(User user, string role)
        {
            var roleDto = new UserRoleUpdateDto {Role = role};
            var expectedUser = user
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(It.Is<User>(u => expectedUser.Equals(u))))
                .ReturnsAsync((User user) => GetUserRoles(user).ToList());

            Func<Task> result = async () => await _sut.UpdateRoleAsync(user.Id, roleDto);

            await result.Should().ThrowAsync<ForumException>()
                .WithMessage($"User '{user.UserName}' already in role '{roleDto.Role}'");

            _userManagerMock.Verify(um =>
                um.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            _userManagerMock.Verify(um =>
                um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUser()
        {
            var user = RegularUser;
            var expectedUser = RegularUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(RegularUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(It.Is<User>(u => expectedUser.Equals(u)), "Administrator"))
                .ReturnsAsync(false);

            await _sut.DeleteAsync(user.Id);

            _userManagerMock.Verify(um =>
                um.DeleteAsync(It.Is<User>(u => expectedUser.Equals(u))), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenUserDoesNotExist()
        {
            var nonexistentUserId = Guid.NewGuid();

            _userManagerMock.Setup(um => um.FindByIdAsync(nonexistentUserId.ToString()))
                .ReturnsAsync((User) null);

            Func<Task> result = async () => await _sut.DeleteAsync(nonexistentUserId);

            await result.Should().ThrowAsync<NotFoundException>();

            _userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenUserIsAdministrator()
        {
            var adminUser = AdminUser;
            var expectedUser = AdminUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByIdAsync(adminUser.Id.ToString()))
                .ReturnsAsync(AdminUser);
            _userManagerMock.Setup(um => um.IsInRoleAsync(It.Is<User>(u => expectedUser.Equals(u)), "Administrator"))
                .ReturnsAsync(true);

            Func<Task> result = async () => await _sut.DeleteAsync(adminUser.Id);

            await result.Should().ThrowAsync<ForumException>();

            _userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<User>()), Times.Never);
        }

        public static IEnumerable<object[]> UpdateRoleAsync_TestData()
        {
            yield return new object[] {RegularUser, "Moderator"};
            yield return new object[] {ModerUser, "User"};
        }

        public static IEnumerable<object[]> UpdateRoleAsync_ShouldFail_TestData()
        {
            yield return new object[] {RegularUser, "User"};
            yield return new object[] {ModerUser, "Moderator"};
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

        private static IEnumerable<(Guid userId, List<string> roles)> UserRolesList =>
            new List<(Guid userId, List<string> roles)>
            {
                (new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"), new List<string> {"User"}),
                (new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"), new List<string> {"Moderator"}),
                (new Guid("6bc56cad-0687-427a-a836-435d25af8575"), new List<string> {"Administrator"})
            };

        private static IEnumerable<UserWithDetailsDto> UserWithDetailsDtoList =>
            new List<UserWithDetailsDto>
            {
                new UserWithDetailsDto
                {
                    Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    Username = "username1",
                    Email = "email1@example.com",
                    Name = "name1",
                    RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12),
                    Roles = new List<string> {"User"}
                },
                new UserWithDetailsDto
                {
                    Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    Username = "username2",
                    Email = "email2@example.com",
                    Name = "name2",
                    RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59),
                    Roles = new List<string> {"Moderator"}
                },
                new UserWithDetailsDto
                {
                    Id = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                    Username = "username3",
                    Email = "email3@example.com",
                    Name = "name3",
                    RegistrationDate = new DateTime(2005, 6, 3, 9, 12, 11),
                    Roles = new List<string> {"Administrator"}
                }
            };

        private static UserProfileInfoDto ExpectedUserProfileInfoDto => new UserProfileInfoDto
        {
            Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
            Username = "username1",
            Name = "name1",
            RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12),
            Roles = new List<string> {"User"}
        };

        private static IEnumerable<string> GetUserRoles(User user)
        {
            return UserRolesList.First(i => i.userId == user.Id).roles;
        }

        private static User GetUserByRole(string role)
        {
            var userRolePair = UserRolesList.First(i => i.roles.Contains(role));
            return UserList.First(u => u.Id == userRolePair.userId);
        }

        private static User RegularUser => GetUserByRole("User");
        private static User ModerUser => GetUserByRole("Moderator");
        private static User AdminUser => GetUserByRole("Administrator");
    }
}