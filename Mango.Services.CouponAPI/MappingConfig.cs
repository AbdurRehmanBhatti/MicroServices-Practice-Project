using AutoMapper;
using Mango.Services.CouponAPI.Modals;
using Mango.Services.CouponAPI.Modals.Dto;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CouponDto, Coupon>();
                cfg.CreateMap<Coupon, CouponDto>();
            });
        }
    }
}
