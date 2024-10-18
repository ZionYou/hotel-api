using Controllers;
using HotelAPI.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly ILogger<HotelInfoController> _logger;
        private readonly HotelContext _context;

        public RoomTypeController(HotelContext context)
        {
            _context = context;
        }

        //得到RoomType全部的資料
        // GET: api/<RoomTypeController>
        [HttpGet]
        public IEnumerable<object> GetRoomType()
        {
            var result = _context.RoomType
                         .Select(a => new
                         {
                             a.TypeName
                         }).ToList();


            return result;
        }

        //得到RoomType指定id的資料
        // GET api/<RoomTypeController>/5
        [HttpGet("{id}")]
        public RoomType Get(int id)
        {
            var result = (from a in _context.RoomType
                          where a.RoomTypeID == id
                          select a)
                          .SingleOrDefault();


            return result;
        }


        // 新增RoomType的資料
        // POST api/<RoomTypeController>
        [HttpPost("InsertRoomType")]
        public async Task<IActionResult> PostRoomType(string type)
        {
            if (type == null || type.Trim().Length == 0)
            {
                return BadRequest("請輸入正確資料");
            }

            RoomType roomtype = new RoomType
            {
                TypeName = type
            };

            _context.RoomType.Add(roomtype);
            await _context.SaveChangesAsync();

            return Ok("插入資料成功");
        }

        // 更新RoomType指定id的TypeName資料
        // PUT api/<RoomTypeController>/5
        [HttpPut("UpdateRoomType/{id}/{type}")]
        public async Task<IActionResult> UpdateRoomType(int id, string type)
        {
            // 检查是否存在
            var result = await _context.RoomType.FindAsync(id);
            if (result == null)
            {
                return BadRequest($"Could not find type {type}");
            }
            else if (type == null || type.Trim().Length == 0)
            {
                return BadRequest("請輸入正確資料");
            }

            // 更新
            result.TypeName = type;

            await _context.SaveChangesAsync();

            return Ok("更新資料成功");
        }

        // 更新RoomType指定id的資料
        // DELETE api/<RoomTypeController>/5
        [HttpDelete("DeleteRoomType/{id}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            // 检查是否存在
            var result = await _context.RoomType.FindAsync(id);
            if (result == null)
            {
                return BadRequest("Could not find type");
            }

            _context.RoomType.Remove(result);
            await _context.SaveChangesAsync();

            return Ok("刪除資料成功");
        }
    }
}
