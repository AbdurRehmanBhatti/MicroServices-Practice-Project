using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Modals;
using Mango.Services.ProductAPI.Modals.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public ProductAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> products = _db.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
            }

            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product coupon = _db.Products.First(x => x.ProductId == id);
                _response.Result = _mapper.Map<ProductDto>(coupon); // Mapping Product to ProductDto
            }
            catch (Exception ex)
            {
                return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
            }

            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ProductDto couponDto)
        {
            try
            {
                _db.Products.Add(_mapper.Map<Product>(couponDto));
                _db.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(couponDto); // Mapping Coupon to CouponDto
            }
            catch (Exception ex)
            {
                return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
            }

            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ProductDto couponDto)
        {
            try
            {
                _db.Products.Update(_mapper.Map<Product>(couponDto));
                _db.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(couponDto); // Mapping Coupon to CouponDto
            }
            catch (Exception ex)
            {
                return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
            }

            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                _db.Products.Remove(_db.Products.First(x => x.ProductId == id));
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
            }

            return _response;
        }
    }
}
