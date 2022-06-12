using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Service.DataTransferObjects;

namespace Service.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<IdentityResult> SignUpAsync(SignUpDto signUpDto);
        public Task<SessionDto> SignInAsync(SingInDto signInDto);
    }
}