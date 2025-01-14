namespace Mango.Services.ShoppingCartAPI.Modals.Dto
{
	public class CartDto
	{
		public CartHeaderDto CartHeader { get; set; }
		public IEnumerable<CartDetailsDto>? CartDetails { get; set; }
	}
}
