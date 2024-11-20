using Mango.Services.AuthAPI.Modals.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var errorMessages = await _authService.RegisterUser(registerDto);
            if(!string.IsNullOrEmpty(errorMessages))
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = errorMessages;
                return BadRequest(_response);
            }

            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResponse = await _authService.Login(loginDto);

            if(loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = "Invalid login attempt";
                return Unauthorized(_response);
            }

            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterDto registerDto)
        {
            var isAssigned = await _authService.AssignRole(registerDto.Email, registerDto.RoleName.ToUpper());
            if(!isAssigned)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = "Failed to assign role";
                return BadRequest(_response);
            }
            _response.Result = isAssigned;
            return Ok(_response);
        }
    }
}
