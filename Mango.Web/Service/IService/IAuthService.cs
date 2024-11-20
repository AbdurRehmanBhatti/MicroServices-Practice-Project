using Mango.Web.Modals;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginDto loginDto);
        Task<ResponseDto?> ResgisterAsync(RegisterDto registerDto);
        Task<ResponseDto?> AssignRoleAsync(RegisterDto registerDto);
    }
}
