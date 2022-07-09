using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Service.DataTransferObjects;
using Service.Exceptions;
using Service.Interfaces;

namespace Service.Services
{
    /// <inheritdoc />
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ISession _session;

        /// <summary>
        /// Constructor for initializing a <see cref="AccountService"/> class instance
        /// </summary>
        /// <param name="userManager">Identity user manager</param>
        /// <param name="mapper">Mapper</param>
        /// <param name="session">Current session</param>
        public AccountService(UserManager<User> userManager, IMapper mapper, ISession session)
        {
            _userManager = userManager;
            _mapper = mapper;
            _session = session;
        }

        public async Task<UserWithDetailsDto> GetInfoAsync()
        {
            var user = await _userManager.FindByIdAsync(_session.UserId.ToString());
            if (user == null)
            {
                throw new ForumException($"User with id {_session.UserId} does not exist");
            }

            var result = _mapper.Map<UserWithDetailsDto>(user);
            result.Roles = await _userManager.GetRolesAsync(user);

            return result;
        }

        public async Task UpdateAsync(AccountUpdateDto accountDto)
        {
            var user = await _userManager.FindByIdAsync(_session.UserId.ToString());
            if (user == null)
            {
                throw new ForumException($"User with id {_session.UserId} does not exist");
            }

            if (!await _userManager.CheckPasswordAsync(user, accountDto.CurrentPassword))
            {
                throw new ForumException($"Incorrect password");
            }

            _mapper.Map(accountDto, user);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var sb = new StringBuilder();
                sb.AppendJoin(' ', result.Errors.Select(e => e.Description));

                throw new ForumException(sb.ToString());
            }
        }

        public async Task ChangePasswordAsync(PasswordChangeDto passwordDto)
        {
            var user = await _userManager.FindByIdAsync(_session.UserId.ToString());
            if (user == null)
            {
                throw new ForumException($"User with id {_session.UserId} does not exist");
            }

            var result = await _userManager.ChangePasswordAsync(user, passwordDto.CurrentPassword, passwordDto.NewPassword);

            if (!result.Succeeded)
            {
                var sb = new StringBuilder();
                sb.AppendJoin(' ', result.Errors.Select(e => e.Description));

                throw new ForumException(sb.ToString());
            }
        }
    }
}