using Mango.Services.ShoppingCartAPI.Modals.Dto;

namespace Mango.Services.ShoppingCartAPI.Service.IService
{
	public interface ICouponService
	{
		Task<CouponDto> GetCoupon(string couponCode);
	}
}
