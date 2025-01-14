using Mango.Services.ShoppingCartAPI.Modals.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Service
{
	public class ProductService : IProductService
	{
		private readonly IHttpClientFactory _clientFactory;

		public ProductService(IHttpClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}
		public async Task<IEnumerable<ProductDto>> GetProducts()
		{
			try
			{
				var client = _clientFactory.CreateClient("Product");
				var response = await client.GetAsync($"/api/product");
				var apiContent = await response.Content.ReadAsStringAsync();
				var responseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
				if (responseDto.IsSuccess)
				{
					return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(responseDto.Result));
				}
				return new List<ProductDto>();
			}
			catch (Exception ex)
			{
				return new List<ProductDto>();
			}
		}
	}
}
