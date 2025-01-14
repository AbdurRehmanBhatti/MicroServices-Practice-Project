using AutoMapper;
using Mango.Services.ShoppingCartAPI.Modals;
using Mango.Services.ShoppingCartAPI.Modals.Dto;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                cfg.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
            });
        }
    }
}
