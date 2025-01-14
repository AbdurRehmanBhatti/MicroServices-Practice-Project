using Mango.Web.Modals;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    /// <summary>
    /// Controller to manage shopping cart operations.
    /// </summary>
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _cartService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCartController"/> class.
        /// </summary>
        /// <param name="cartService">The shopping cart service.</param>
        public ShoppingCartController(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Displays the shopping cart index view.
        /// </summary>
        /// <returns>The shopping cart index view.</returns>
        [Authorize]
        public async Task<IActionResult> ShoppingCartIndex()
        {
            return View(await LoadShoppingCartBasedOnLoggedInUser());
        }

        /// <summary>
        /// Loads the shopping cart based on the logged-in user.
        /// </summary>
        /// <returns>The shopping cart data transfer object.</returns>
        private async Task<CartDto> LoadShoppingCartBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }
            return new CartDto();
        }
    }
}