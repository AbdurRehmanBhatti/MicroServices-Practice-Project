using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Modals;
using Mango.Services.AuthAPI.Modals.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    /// <summary>
    /// Service for handling authentication-related operations.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="db">The application database context.</param>
        /// <param name="userManager">The ASP.NET identity user manager.</param>
        /// <param name="roleManager">The ASP.NET identity role manager.</param>
        /// <param name="jwtTokenGenerator">The JWT token generator.</param>
        public AuthService(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="roleName">The name of the role to assign.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the role was successfully assigned.</returns>
        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    // Create the role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }

                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginDto">The login data transfer object.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the login response data transfer object.</returns>
        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginDto.UserName.ToLower());
            bool IsValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (user == null || !IsValid)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }

            // If user is valid, generate token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDto = new()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponse = new()
            {
                User = userDto,
                Token = token
            };

            return loginResponse;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDto">The registration data transfer object.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a string indicating the result of the registration.</returns>
        public async Task<string> RegisterUser(RegisterDto registerDto)
        {
            ApplicationUser user = new()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                NormalizedEmail = registerDto.Email.ToUpper(),
                Name = registerDto.Name,
                PhoneNumber = registerDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == registerDto.Email);

                    UserDto userDto = new()
                    {
                        Id = userToReturn.Id,
                        Name = userToReturn.Name,
                        Email = userToReturn.Email,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }

            return "Error Encountered";
        }
    }
}