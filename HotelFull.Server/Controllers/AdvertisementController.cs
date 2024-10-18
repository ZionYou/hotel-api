using Controllers;
using HotelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Server.Service;

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    public class AdvertisementController : Controller
    {
        private readonly ILogger<HotelInfoController> _logger;
        private readonly HotelContext _context;
        private readonly ImageService _imageService;
        private readonly NewsService _newsService;

        public AdvertisementController(HotelContext context)
        {
            _context = context;
        }

        [HttpGet("AdvertisementInfo")]
        public IActionResult GetAdvertisementInfo()
        {
            var res = (from ad in _context.Advertisement
                       join img in _context.Image on ad.ImageID equals img.ImageID into imgJoin
                       from img in imgJoin.DefaultIfEmpty()
                       where img == null || img.ImageTypeID == 1
                       orderby ad.Adpriority
                       select new
                       {
                           ad.AdvertisementID,
                           img.ImageID,
                           ad.Title,
                           ad.StartDate,
                           ad.EndDate,
                           ad.URL,
                           ad.Adpriority,
                           ImageBase64 = img != null
                               ? Convert.ToBase64String(img.ImageBinary)
                               : null
                       }).ToList();

            if (res == null || !res.Any())
            {
                return BadRequest("No advertisements found.");
            }

            return Ok(res);
        }


        //取得所有Advertisement image order by priority asc for 輪播系統
        [HttpGet("AdvertisementForBanner")]
        public IActionResult AdvertisementGorBanner()
        {
            
            var res = (from ad in _context.Advertisement
                       where (ad.StartDate <= DateTime.Now || ad.StartDate == DateTime.MinValue)
                            && (ad.EndDate >= DateTime.Now || ad.EndDate == DateTime.MinValue)
                       join img in _context.Image on ad.ImageID equals img.ImageID into imgJoin
                       from img in imgJoin.DefaultIfEmpty()
                       where img == null || img.ImageTypeID == 1
                       orderby ad.Adpriority
                       select new
                       {
                           ImageBase64 = img != null
                               ? Convert.ToBase64String(img.ImageBinary)
                               : null
                       }).ToList();
            return Ok(res);
        }


        [HttpPost]
        public async Task<IActionResult> AddAdvertisement(
                                                            [FromForm] string title,
                                                            [FromForm] DateTime StartDate,
                                                            [FromForm] DateTime EndDate,
                                                            [FromForm] int Adpriority,
                                                            [FromForm] string status,
                                                            [FromForm] string URL,
                                                            IFormFile img)
        {
            if (img == null || img.Length == 0)
            {
                return BadRequest("圖片文件不可為空");
            }

            try
            {
                // 將 IFormFile 轉換為 byte[]
                byte[] imageData;
                using (var ms = new MemoryStream())
                {
                    await img.CopyToAsync(ms);
                    imageData = ms.ToArray();
                }

                // 建立 Image 實體
                Image newImage = new Image
                {
                    ImageBinary = imageData,
                    ImageTypeID = 1 // 根據需求可以調整 ImageTypeID
                };

                _context.Image.Add(newImage);
                await _context.SaveChangesAsync(); // 儲存 Image 並獲取 ImageID

                // 建立 Advertisement 實體
                Advertisement newAdvertisement = new Advertisement
                {
                    Title = title,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    URL = URL,
                    Adpriority = Adpriority,
                    ImageID = newImage.ImageID  // 將儲存後的 ImageID 設定為外鍵
                };

                _context.Advertisement.Add(newAdvertisement);
                await _context.SaveChangesAsync(); // 儲存 Advertisement

                return Ok(new { message = "廣告新增成功", advertisement = newAdvertisement });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"內部伺服器錯誤: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAdvertisement([FromForm] int id, [FromForm] string title, [FromForm] DateTime StartDate,
                                                [FromForm] DateTime EndDate, [FromForm] int Adpriority, [FromForm] string status, [FromForm] string URL,  IFormFile img)
        {

            var res = _context.Advertisement.FirstOrDefault(ad => ad.AdvertisementID == id);

            if(res == null)
            {
                return BadRequest("error find advertisement");
            }

            res.Title = title;
            res.StartDate = StartDate;
            res.EndDate = EndDate;
            res.URL = URL;
            res.Adpriority = Adpriority;

            await _context.SaveChangesAsync();

            return Ok($"update success: {res}");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAdvertisement([FromForm] int id)
        {
            var res = _context.Advertisement.FirstOrDefault(ad => ad.AdvertisementID == id);

            if(res == null)
            {
                return BadRequest($"error find advertisement: {id}");
            }

            var imageId = res.ImageID;
            var i = _context.Image.FirstOrDefault(i => i.ImageID == imageId);

            _context.Advertisement.Remove(res);
            _context.Image.Remove(i);

           await _context.SaveChangesAsync();
            return Ok($"delete success");
        }


    }
}
