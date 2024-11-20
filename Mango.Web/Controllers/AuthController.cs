using Mango.Web.Modals;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        /// <param name="tokenProvider">The token provider.</param>
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        /// <summary>
        /// Displays the login view.
        /// </summary>
        /// <returns>The login view.</returns>
        [HttpGet]
        public IActionResult Login()
        {
            LoginDto loginDto = new();
            return View(loginDto);
        }

        /// <summary>
        /// Handles the login form submission.
        /// </summary>
        /// <param name="loginDto">The login data.</param>
        /// <returns>The result of the login attempt.</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            ResponseDto responseDto = await _authService.LoginAsync(loginDto);

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                await SignInUser(loginResponseDto);

                _tokenProvider.SetToken(loginResponseDto.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = responseDto.ErrorMessage;
                return View(loginDto);
            }
        }

        /// <summary>
        /// Displays the registration view.
        /// </summary>
        /// <returns>The registration view.</returns>
        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>
                {
                    new SelectListItem() { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                    new SelectListItem() { Text = SD.RoleCustomer, Value = SD.RoleCustomer }
                };

            ViewBag.RoleList = roleList;

            return View();
        }

        /// <summary>
        /// Handles the registration form submission.
        /// </summary>
        /// <param name="registerDto">The registration data.</param>
        /// <returns>The result of the registration attempt.</returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            ResponseDto responseDto = await _authService.ResgisterAsync(registerDto);
            ResponseDto assignRole;

            if (responseDto != null && responseDto.IsSuccess)
            {
                if (string.IsNullOrEmpty(registerDto.RoleName))
                {
                    registerDto.RoleName = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(registerDto);
                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = responseDto.ErrorMessage;
            }

            var roleList = new List<SelectListItem>
                {
                    new SelectListItem() { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                    new SelectListItem() { Text = SD.RoleCustomer, Value = SD.RoleCustomer }
                };

            ViewBag.RoleList = roleList;

            return View(registerDto);
        }

        /// <summary>
        /// Displays the logout view.
        /// </summary>
        /// <returns>The logout view.</returns>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.RemoveToken();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Signs in a user with the provided login response.
        /// </summary>
        /// <param name="loginDto">The login response containing the token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SignInUser(LoginResponseDto loginDto)
        {
            var handler = new JwtSecurityTokenHandler();

            var jsonToken = handler.ReadJwtToken(loginDto.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jsonToken.Claims.FirstOrDefault(c => c.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}