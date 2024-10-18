using HotelAPI.Service;
using HotelFull.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Route("api/[controller]")]
    public class HotelInfoController : ControllerBase
    {
        private readonly ILogger<HotelInfoController> _logger;
        private readonly HotelContext _context;
        private readonly HotelService _hotelService;


        public HotelInfoController(HotelContext context, HotelService hotelService)
        {
            _context = context;
            _hotelService = hotelService; // 注入 HotelService
        }

        //依照目前時間及visable屬性取得News
        [HttpGet]
        public IActionResult GethotelInfo()
        {
            var res = _context.HotelInfo.Select(i => new
            {
                i.HotelInfoID,
                i.HotelName,
                i.FullAddress,
                i.Longitude,
                i.Latitude,
                i.Phone,
                i.Email,
                i.Description,
                i.CheckInTime,
                i.CheckOutTime,
            }).ToList();

            return Ok(res);
        }

        

        [HttpPut]
        public async Task<IActionResult> PosthotelInfoAsync(
        [FromBody] HotelInfoRequest request)
        {
            Console.WriteLine("address: " + request.Address);

            if (string.IsNullOrEmpty(request.Address))
            {
                return BadRequest("Address cannot be null or empty.");
            }

            var hi = _context.HotelInfo.FirstOrDefault();

            if (hi != null)
            {
                hi.HotelName = request.Name;
                hi.Phone = request.Phone;
                hi.Email = request.Email;
                hi.Description = request.Desc;
                hi.CheckInTime = request.CheckIn;
                hi.CheckOutTime = request.CheckOut;

                var coordinates = await _hotelService.GeocodeAsync(request.Address);

                if (coordinates.HasValue)
                {
                    hi.Latitude = coordinates.Value.lat;
                    hi.Longitude = coordinates.Value.lng;
                    hi.FullAddress = request.Address;

                    await _context.SaveChangesAsync();
                    return Ok("修改完成");
                }
                else
                {
                    return BadRequest("地址有誤,更新失敗");
                }
            }

            return NotFound("出問題囉");
        }

    }
}

