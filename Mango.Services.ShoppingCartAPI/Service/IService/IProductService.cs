using Mango.Services.ShoppingCartAPI.Modals.Dto;

namespace Mango.Services.ShoppingCartAPI.Service.IService
{
	public interface IProductService
	{
		Task<IEnumerable<ProductDto>> GetProducts();
	}
}