using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelAPI.Models;
using HotelAPI.Service;
using HotelFull.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using test.Server.Service;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly ILogger<HotelInfoController> _logger;
        private readonly HotelContext _context;
        public ProductTypeController(HotelContext context)
        {
            _context = context;
        }


        //取得所有ProductType
        [HttpGet]
        public IActionResult GetProductTypeInfo()
        {
            Console.WriteLine("-----------------------in AllProductType-----------------------");
            var res = _context.ProductType.ToList();
            Console.WriteLine("-----------------------out AllProductType-----------------------");
            return Ok(res);
        }

        //取得所有ProductType
        [HttpGet("{id}")]
        public IActionResult GetProductTypeInfoById(int id)
        {
            var res = _context.ProductType
                .Where(i => i.ProductTypeID == id)
                .Select(i => new
            {
                i.ProductTypeName
            }).ToList();

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> InsertProductType([FromBody] ProductTypeRequest request)
        {
            if(request.type == null || request.type.Trim().Length == 0)
            {
                return BadRequest("請輸入正確資料");
            }
            ProductType p = new ProductType
            {
                ProductTypeName = request.type
            };

            _context.ProductType.Add(p);
            await _context.SaveChangesAsync();

            return Ok("插入資料成功");
        }

        //update ProductType
        [HttpPut]
        public async Task<IActionResult> UpdateProductType([FromBody] ProductTypeRequest request)
        {
            // 检查是否存在
            var ty = await _context.ProductType.FindAsync(request.id);
            if(ty == null)
            {
                return BadRequest($"Could not find type {request.type}");
            }
            else if (request.type == null || request.type.Trim().Length == 0)
            {
                return BadRequest("請輸入正確資料");
            }

            // 更新
            ty.ProductTypeName = request.type;

            await _context.SaveChangesAsync();

            return Ok("更新資料成功");
        }

        //delete ProductType
        [HttpDelete]
        public async Task<IActionResult> DeleteProductType([FromBody] int id)
        {
            // 检查是否存在
            var ty = await _context.ProductType.FindAsync(id);
            if (ty == null)
            {
                return BadRequest("Could not find type");
            }

            _context.ProductType.Remove(ty);
            await _context.SaveChangesAsync();

            return Ok("刪除資料成功");
        }
    }
}

