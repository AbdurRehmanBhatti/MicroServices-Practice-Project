using Mango.Services.AuthAPI.Modals.Dto;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> RegisterUser(RegisterDto registerDto);
        Task<LoginResponseDto> Login(LoginDto loginDto);
        Task<bool> AssignRole (string email, string roleName);
    }
}