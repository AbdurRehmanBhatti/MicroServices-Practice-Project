using Mango.Web.Modals;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _service;
        public ProductService(IBaseService service) 
        {
            _service = service; 
        }
        public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
        {
            return await _service.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = productDto,
                Url = SD.ProductApiBase + "/api/product/"
            });
        }

        public async Task<ResponseDto?> DeleteProductAsync(int id)
        {
            return await _service.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ProductApiBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDto?> GetAllProductsAsync()
        {
            return await _service.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductApiBase + "/api/product/"
            });
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return await _service.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductApiBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
        {
            return await _service.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = productDto,
                Url = SD.ProductApiBase + "/api/product/"
            });
        }
    }
}
