using HotelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Controllers
{
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<HotelInfoController> _logger;
        private readonly HotelContext _context;
        ImageService imageService = new ImageService();
        public ProductController(HotelContext context)
        {
            _context = context;
        }

        //get all product
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Product
                            .Include(p => p.Image)  // 加載 Image 導航屬性
                             .Select(p => new
                             {
                                 p.ProductID,
                                 p.ProductName,
                                 p.Price,
                                 Image = p.Image != null
                                     ? imageService.ReturnPhoto(p.Image.ImageBinary)  // 直接調用 ReturnPhoto
                                     : null
                             })
                             .ToList();
            if (products == null || products.Count == 0)
            {
                return BadRequest("");
            }
            return Ok(products);
        }

        //get products by type id
        [HttpGet("Product/{typeId}")]
        public IActionResult GetProductsByType(int typeId)
        {
            var products = _context.Product
                                     .Where(p => p.ProductTypeID == typeId) // 根據 typeId 進行過濾
                                     .Include(p => p.Image)  // 加載 Image 導航屬性
                                     .Select(p => new
                                     {
                                         p.ProductID,
                                         p.ProductName,
                                         p.Price,
                                         ImageBase64 = p.Image != null
                                             ? Convert.ToBase64String(p.Image.ImageBinary) // 直接調用 ReturnPhoto
                                             : null,
                                         p.ProductTypeID
                                     })
                                     .ToList();
            if(products == null)
            {
                return BadRequest("null");
            }
            return Ok(products);
        }

        [HttpGet("Product/single/{id}")]
        public IActionResult GetProductsById(int id)
        {
            var products = _context.Product
                .Where(p => p.ProductID == id)  // 根据 ProductID 进行过滤
                .Include(p => p.Image)  // 加载 Image 导航属性
                .Include(p => p.ProductType)  // 加载 ProductType 导航属性
                .Select(p => new
                {
                    p.ProductID,
                    p.ProductName,
                    p.Price,
                    p.Status,
                    ProductTypeName = p.ProductType.ProductTypeName,  // 从 ProductType 对象中获取名称
                })
                .ToList();

            if (products == null || !products.Any())  // 检查 products 是否为空或没有任何结果
            {
                return NotFound("No products found with the given ID");
            }

            return Ok(products);
        }


        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] string name , [FromForm] int price , [FromForm] string status, 
            [FromForm] int typeId ,  IFormFile img
            )
        {
            var ms = new MemoryStream();
            await img.CopyToAsync(ms); // 將 IFormFile 轉換為 MemoryStream
            var imageData = ms.ToArray(); // 將 MemoryStream 轉換為 byte[]
            //------ Image insert start -----
            Image newImage = new Image
            {
                ImageBinary = imageData,
                ImageTypeID = 2 // 根據需求可以調整 ImageTypeID
            };

            _context.Image.Add(newImage);
            await _context.SaveChangesAsync();  // 先保存，讓 ImageID 自動生成
            //------ Image insert end -----

            //------ Product insert start -----
            Product newProduct = new Product
            {
                ProductName = name,
                Price = price,
                Status = status,
                ProductTypeID = typeId,
                ImageID = newImage.ImageID // 設定 ImageID 為剛剛插入的 Image 的 ID
            };

            _context.Product.Add(newProduct);
            await _context.SaveChangesAsync();  // 保存 Product 資料
                                                //------ Product insert end -----

            return Ok("商品新增成功");
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromForm] string name, [FromForm] decimal price, [FromForm] string status
            )
        {
           Product p = _context.Product.FirstOrDefault(x => x.ProductID == id);
            if(p == null)
            {
                return BadRequest("product is null");
            }
            else
            {
                p.ProductName = name;
                p.Price =  price;
                p.Status = status;
            }
            await _context.SaveChangesAsync();
            return Ok("資料Insert 完成");
        }

        [HttpDelete]
        public async Task<IActionResult> deleteProduct([FromBody] int id)
        {
            Product p = _context.Product.FirstOrDefault(i => i.ProductID == id);
            Image I = _context.Image.FirstOrDefault(x => x.ImageID == p.ImageID);
            if (p == null)
            {
                return BadRequest("cant find this product");
            }
            else
            {
                //刪除Product
                _context.Product.Remove(p);

                //刪除Product
                _context.Image.Remove(I);

               await _context.SaveChangesAsync();
            }
            return Ok("delete success");
        }

    }
}

