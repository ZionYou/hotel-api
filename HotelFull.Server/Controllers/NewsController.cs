using HotelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Server.Service;

namespace Controllers
{
    [Route("api/[controller]")]
    public class NewsController : Controller
    {
        private readonly HotelContext _context;
        private NewsService ns = new NewsService();

        public NewsController(HotelContext context)
        {
            _context = context;
        }

        //依照目前時間及visable屬性取得News
        [HttpGet("AllNewsForEnd")]
        public IActionResult AllNewsForEnd()
        {
            var res = _context.News
                                .OrderByDescending(i => i.publishDate)
                                .Select(i => new { i.id, i.subject, i.publishDate })
                                .ToList();

            return Ok(res);
        }

        //依照目前時間及visable屬性取得News
        [HttpGet]
        public IActionResult GetAllNewsSubject()
        {
            var res = _context.News
                                .Where(i => i.visable && i.publishDate <= ns.getToday())
                                .OrderByDescending(i => i.publishDate)
                                .Select(i => new { i.id , i.subject, i.publishDate })
                                .ToList();

            return Ok(res);
        }

        //依照id屬性取得News content
        [HttpGet("{id}")]
        public IActionResult GetNewsContentById(int id)
        {
            // 查找指定 id 的新闻
            var news = _context.News
                               .Where(i => i.id == id)
                               .FirstOrDefault();

            // 如果新闻不存在，返回 404 错误
            if (news == null)
            {
                return NotFound("找不到該新聞");
            }

            // 返回新闻内容
            return Ok(news);
        }



        [HttpPost]
        public async Task<IActionResult> PostNews([FromBody] NewsDto newsDto)
        {
            if (string.IsNullOrWhiteSpace(newsDto.Subject))
            {
                return BadRequest("主旨不可為空");
            }

            if (string.IsNullOrWhiteSpace(newsDto.Content))
            {
                return BadRequest("內容不可為空");
            }

            if (newsDto.PublishDate < DateTime.Today) // 使用 DateTime.Today 獲取今天的日期
            {
                return BadRequest("日期怪怪的,你要不要再確認一下");
            }

            News n = new News
            {
                subject = newsDto.Subject,
                content = newsDto.Content,
                publishDate = newsDto.PublishDate,
                visable = newsDto.Visable
            };

            _context.News.Add(n);
            await _context.SaveChangesAsync();

            return Ok(n);
        }


        public class NewsDto
        {
            public string Subject { get; set; }
            public string Content { get; set; }
            public DateTime PublishDate { get; set; }
            public bool Visable { get; set; }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PgutNews(int id, [FromBody] string subject, [FromBody] string content, [FromBody] DateTime publishDate, [FromBody] bool visable)
        {
            // 检查新闻是否存在
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound("找不到該新聞");
            }

            // 检查主旨是否为空
            if (string.IsNullOrWhiteSpace(subject))
            {
                return BadRequest("主旨不可為空");
            }

            // 检查内容是否为空
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("內容不可為空");
            }

            // 检查发布日期
            if (publishDate < ns.getToday())
            {
                return BadRequest("日期怪怪的,你要不要再確認一下");
            }

            // 更新新闻的字段
            news.subject = subject;
            news.content = content;
            news.publishDate = publishDate;
            news.visable = visable;

            try
            {
                // 保存更改
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
                {
                    return NotFound("找不到該新聞");
                }
                else
                {
                    throw;
                }
            }

            // 返回成功响应
            return Ok(news);
        }

        // delete News
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            if (!NewsExists(id))
            {
                return BadRequest("找不到此新聞,請再次確認");
            }

            var news = _context.News.FirstOrDefault(n => n.id == id);
            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
            }

            return Ok("delete complete");
        }

        // 检查新闻是否存在的方法
        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.id == id);
        }




    }
}
