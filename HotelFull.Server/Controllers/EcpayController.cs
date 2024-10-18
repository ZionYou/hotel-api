


using HotelAPI.Service;
using HotelFull.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text;

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    public class EcpayController : Controller
    {
        private readonly ILogger<EcpayController> _logger;
        private readonly HotelContext _context;
        private readonly EcpayService _ecpayService;

        public EcpayController(HotelContext context , EcpayService ecpayService)
        {
            _context = context;
            _ecpayService = ecpayService;  
        }

        [HttpPost("goToEcpay")]
        public IActionResult goToEcpay([FromBody] EcpayRequest request)
        {
            Dictionary<string, string> body = new Dictionary<string, string>();

            Dictionary<string, string> prodlist = request.prodlist;

            if(prodlist == null ||  prodlist.Count == 0)
            {
                return BadRequest("no product to get");
            }
            body = _ecpayService.getBody(request.title , prodlist);

            return Ok(body);
        }

        [HttpPost("fetchEcpay")]
        public IActionResult fetchEcpay()
        {
            var formData = Request.Form;

            // 将参数和对应的值格式化为字符串
            StringBuilder resultBuilder = new StringBuilder();
            foreach (var key in formData.Keys)
            {
                resultBuilder.AppendLine($"{key} = {formData[key]}");
            }

            // 输出到控制台
            Console.WriteLine(resultBuilder.ToString());
            // 返回响应
            return Ok("resultBuilder.ToString(): " +resultBuilder.ToString());
        }


        public class EcpayRequest
        {
           public string title {  get; set; }
           public Dictionary<string, string> prodlist {  get; set; }
        }
    }
}









