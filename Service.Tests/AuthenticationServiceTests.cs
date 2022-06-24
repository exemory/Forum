using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data.Entities;
using ExpectedObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Service.DataTransferObjects;
using Service.Exceptions;
using Service.Services;
using Xunit;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Service.Tests
{
    public class AuthenticationServiceTests
    {
        private readonly AuthenticationService _sut;

        private readonly Mock<UserManager<User>> _userManagerMock =
            new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        public AuthenticationServiceTests()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(ConfigurationSettings)
                .Build();

            _sut = new AuthenticationService(_userManagerMock.Object, UnitTestHelper.CreateMapper(), config);
        }

        [Fact]
        public async Task SignUpAsync_ShouldRegisterUser()
        {
            var signUpDto = TestSignUpDto;
            var expectedUser = CreatedUserToBeSaved
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um =>
                    um.CreateAsync(It.Is<User>(u => expectedUser.Equals(u)), signUpDto.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.Is<User>(u => expectedUser.Equals(u)), "User"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _sut.SignUpAsync(signUpDto);

            result.Should().BeEquivalentTo(IdentityResult.Success);

            _userManagerMock.Verify(um =>
                um.CreateAsync(It.Is<User>(u => expectedUser.Equals(u)), signUpDto.Password), Times.Once);
            _userManagerMock.Verify(um =>
                um.AddToRoleAsync(It.Is<User>(u => expectedUser.Equals(u)), "User"), Times.Once);
        }

        [Fact]
        public async Task SignUpAsync_ShouldReturnError_WhenCreatingUserFailed()
        {
            var signUpDto = TestSignUpDto;
            var expectedUser = CreatedUserToBeSaved
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um =>
                    um.CreateAsync(It.Is<User>(u => expectedUser.Equals(u)), signUpDto.Password))
                .ReturnsAsync(IdentityResult.Failed());

            var result = await _sut.SignUpAsync(signUpDto);

            result.Should().BeEquivalentTo(IdentityResult.Failed());

            _userManagerMock.Verify(um =>
                um.CreateAsync(It.Is<User>(u => expectedUser.Equals(u)), signUpDto.Password), Times.Once);
            _userManagerMock.Verify(um =>
                um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(SignInAsync_TestData))]
        public async Task SignInAsync_ShouldAuthenticateUser_WhenCredentialsAreCorrect(SingInDto signInDto)
        {
            var expectedUserToCheckPassword = TestUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));
            var expectedTokenExpirationDate = DateTime.UtcNow + TimeSpan.Parse(ConfigurationSettings["Jwt:Lifetime"]);
            
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string username) => TestUser.UserName == username ? TestUser : null);
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) => TestUser.Email == email ? TestUser : null);
            _userManagerMock.Setup(um =>
                    um.CheckPasswordAsync(It.Is<User>(u => expectedUserToCheckPassword.Equals(u)), signInDto.Password))
                .ReturnsAsync(true);
            _userManagerMock.Setup(um => um.GetRolesAsync(It.Is<User>(u => expectedUserToCheckPassword.Equals(u))))
                .ReturnsAsync(TestUserRoles.ToList());

            var result = await _sut.SignInAsync(signInDto);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

            result.UserId.Should().Be(TestUser.Id);
            result.Username.Should().Be(TestUser.UserName);
            result.UserRoles.Should().BeEquivalentTo(TestUserRoles);

            jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Iss).Value
                .Should().Be(ConfigurationSettings["Jwt:Issuer"]);
            jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Aud).Value
                .Should().Be(ConfigurationSettings["Jwt:Audience"]);
            jwt.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value
                .Should().Be(TestUser.Id.ToString());
            jwt.Claims.Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .Should().BeEquivalentTo(TestUserRoles);
            
            var tokenExpiration = jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value;
            var tokenExpirationDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(tokenExpiration)).UtcDateTime;
            tokenExpirationDate.Should().BeCloseTo(expectedTokenExpirationDate, TimeSpan.FromSeconds(1));

            _userManagerMock.Verify(um =>
                    um.CheckPasswordAsync(It.Is<User>(u => expectedUserToCheckPassword.Equals(u)), signInDto.Password),
                Times.Once);
        }

        [Fact]
        public async Task SignInAsync_ShouldFail_WhenUserDoesNotExist()
        {
            var signInDto = TestSingInDto;

            _userManagerMock.Setup(um => um.FindByNameAsync(TestSingInDto.Login))
                .ReturnsAsync((User) null);
            _userManagerMock.Setup(um => um.FindByEmailAsync(TestSingInDto.Login))
                .ReturnsAsync((User) null);

            Func<Task> result = async () => await _sut.SignInAsync(signInDto);

            await result.Should().ThrowAsync<AuthenticationException>();

            _userManagerMock.Verify(um =>
                um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SignInAsync_ShouldFail_WhenPasswordIsInvalid()
        {
            var signInDto = TestSingInDto;
            var expectedUserToCheckPassword = TestUser
                .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

            _userManagerMock.Setup(um => um.FindByNameAsync(TestSingInDto.Login))
                .ReturnsAsync(TestUser);
            _userManagerMock.Setup(um => um.FindByEmailAsync(TestSingInDto.Login))
                .ReturnsAsync((User) null);
            _userManagerMock.Setup(um =>
                    um.CheckPasswordAsync(It.Is<User>(u => expectedUserToCheckPassword.Equals(u)), signInDto.Password))
                .ReturnsAsync(false);

            Func<Task> result = async () => await _sut.SignInAsync(signInDto);

            await result.Should().ThrowAsync<AuthenticationException>();

            _userManagerMock.Verify(um =>
                    um.CheckPasswordAsync(It.Is<User>(u => expectedUserToCheckPassword.Equals(u)), signInDto.Password),
                Times.Once);
        }

        public static IEnumerable<object[]> SignInAsync_TestData()
        {
            yield return new object[]
            {
                new SingInDto
                {
                    Login = TestUser.UserName,
                    Password = "test_password_123"
                }
            };
            yield return new object[]
            {
                new SingInDto
                {
                    Login = TestUser.Email,
                    Password = "test_password_123"
                }
            };
        }

        private static Dictionary<string, string> ConfigurationSettings =>
            new Dictionary<string, string>
            {
                {"Jwt:Secret", "fWKzCrKpnldm0lSh"},
                {"Jwt:Issuer", "Test issuer"},
                {"Jwt:Audience", "Test audience"},
                {"Jwt:Lifetime", "01.00:00"},
            };

        private static SignUpDto TestSignUpDto => new SignUpDto
        {
            UserName = "test1",
            Email = "test1@ukr.net",
            Name = "test1",
            Password = "test_password_123"
        };

        private static User CreatedUserToBeSaved => new User
        {
            UserName = "test1",
            Email = "test1@ukr.net",
            Name = "test1"
        };

        private static User TestUser => new User
        {
            Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
            UserName = "test_login",
            Email = "test@example.com",
            Name = "test",
            RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
        };

        private static IEnumerable<string> TestUserRoles = new List<string> {"TestRole"};

        private static SingInDto TestSingInDto => new SingInDto
        {
            Login = "test_login",
            Password = "test_password_123"
        };
    }
}