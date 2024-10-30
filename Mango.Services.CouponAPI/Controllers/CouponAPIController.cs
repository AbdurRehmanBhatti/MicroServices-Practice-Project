using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Modals;
using Mango.Services.CouponAPI.Modals.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public CouponAPIController(ApplicationDbContext db, IMapper mapper)
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
                IEnumerable<Coupon> coupons = _db.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(coupons);
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
                Coupon coupon = _db.Coupons.First(x => x.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(coupon); // Mapping Coupon to CouponDto
            }
            catch (Exception ex)
            {
                return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
            }

            return _response;
        }
        
        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon coupon = _db.Coupons.First(x => x.CouponCode.ToLower() == code.ToLower());
                if(coupon == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = _mapper.Map<CouponDto>(coupon); // Mapping Coupon to CouponDto
            }
            catch (Exception ex)
            {
                return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
            }

            return _response;
        }
        
        [HttpPost]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                _db.Coupons.Add(_mapper.Map<Coupon>(couponDto));
                _db.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(couponDto); // Mapping Coupon to CouponDto
            }
            catch (Exception ex)
            {
                return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
            }

            return _response;
        }
        
        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                _db.Coupons.Update(_mapper.Map<Coupon>(couponDto));
                _db.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(couponDto); // Mapping Coupon to CouponDto
            }
            catch (Exception ex)
            {
                return new ResponseDto { ErrorMessage = ex.Message, IsSuccess = false };
            }

            return _response;
        }
        
        [HttpDelete]
        [Route("{id:int}")]
        public ResponseDto Delete(int id)
        {
            try
            {
                _db.Coupons.Remove(_db.Coupons.First(x => x.CouponId == id));
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
