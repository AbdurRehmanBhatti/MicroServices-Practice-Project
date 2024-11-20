using AutoMapper;
using Mango.Services.ProductAPI.Modals;
using Mango.Services.ProductAPI.Modals.Dto;

namespace Mango.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductDto, Product>().ReverseMap();
            });
        }
    }
}
