using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Modals;
using Mango.Services.ShoppingCartAPI.Modals.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
	[Route("api/cart")]
	[ApiController]
	public class ShoppingCartAPIController : ControllerBase
	{
		private readonly ApplicationDbContext _db;
		private IProductService _productService;
		private ICouponService _couponService;
		private ResponseDto _response;
		private IMapper _mapper;

		public ShoppingCartAPIController(ApplicationDbContext db, IMapper mapper, IProductService productService, ICouponService couponService)
		{
			_db = db;
			_response = new ResponseDto();
			_mapper = mapper;
			_productService = productService;
			_couponService = couponService;
		}

		[HttpGet("GetCart/{userId}")]
		public async Task<ResponseDto> GetCart(string userId)
		{
			try
			{
				CartDto cartDto = new()
				{
					CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId == userId))
				};
				cartDto.CartDetails = _mapper.Map<List<CartDetailsDto>>(_db.CartDetails.Where(u => u.CartHeaderId == cartDto.CartHeader.CartHeaderId));

				IEnumerable<ProductDto> productList = await _productService.GetProducts();

				foreach (var item in cartDto.CartDetails)
				{
					item.Product = productList.FirstOrDefault(u => u.ProductId == item.ProductId);
					cartDto.CartHeader.CartTotal += (item.Product.Price * item.Count);
				}

				//apply card if any
				if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                    if (coupon != null && cartDto.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cartDto.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cartDto.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

				_response.Result = cartDto;
			}
			catch (Exception ex)
			{
				return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
			}
			return _response;
		}

		[HttpPost("ApplyCoupon")]
		public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
		{
			try
			{
				var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
				cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
				_db.CartHeaders.Update(cartFromDb);
				await _db.SaveChangesAsync();
				_response.Result = true;
			}
			catch (Exception ex)
			{
				return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
			}
			return _response;
		}

		[HttpPost("RemoveCoupon")]
		public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
		{
			try
			{
				var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
				cartFromDb.CouponCode = "";
				_db.CartHeaders.Update(cartFromDb);
				await _db.SaveChangesAsync();
				_response.Result = true;
			}
			catch (Exception ex)
			{
				return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
			}
			return _response;
		}

		[HttpPost("CartUpsert")]
		public async Task<ResponseDto> Upsert(CartDto cartDto)
		{
			try
			{
				var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
				if (cartHeaderFromDb == null)
				{
					CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
					_db.CartHeaders.Add(cartHeader);
					await _db.SaveChangesAsync();
					cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
					_db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
					await _db.SaveChangesAsync();
				}
				else
				{
					var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetails.First().ProductId && u.CartDetailsId == cartHeaderFromDb.CartHeaderId);
					if (cartDetailsFromDb == null)
					{
						cartDto.CartHeader.CartHeaderId = cartHeaderFromDb.CartHeaderId;
						_db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
						await _db.SaveChangesAsync();
					}
					else
					{
						cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
						cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
						cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
						_db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
						await _db.SaveChangesAsync();
					}
				}
				_response.Result = cartDto;
			}
			catch (Exception ex)
			{
				return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
			}
			return _response;
		}

		[HttpPost("RemoveCart")]
		public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
		{
			try
			{
				var cartDetails = await _db.CartDetails.FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);

				int totalCountOfCartItems = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
				_db.CartDetails.Remove(cartDetails);
				if (totalCountOfCartItems == 1)
				{
					var cartHeader = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
					_db.CartHeaders.Remove(cartHeader);
				}
				await _db.SaveChangesAsync();

				_response.Result = true;
			}
			catch (Exception ex)
			{
				return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
			}
			return _response;
		}
	}
}
