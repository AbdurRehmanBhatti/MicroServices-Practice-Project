using Mango.Web.Modals;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegisterDto registerDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registerDto,
                Url = SD.AuthApiBase + "/api/auth/assignrole"
            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginDto loginDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginDto,
                Url = SD.AuthApiBase + "/api/auth/login"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> ResgisterAsync(RegisterDto registerDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registerDto,
                Url = SD.AuthApiBase + "/api/auth/register"
            }, withBearer: false);
        }
    }
}
