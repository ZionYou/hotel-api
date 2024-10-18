using Controllers;
using HotelAPI.Models;
using HotelFull.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using static ProductOrder;

namespace HotelFull.Server.Controllers
{
    [Route("api/[controller]")]
    public class ProductOrderController : Controller
    {
        private readonly ILogger<HotelInfoController> _logger;
        private readonly HotelContext _context;

        public ProductOrderController(HotelContext context)
        {
            _context = context;
        }

        //得到所有訂單的資料 (條列式,沒有商品明細)
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.ProductOrder
                .AsNoTracking()
                .Select(po => new
                {
                    po.OrderID,
                    po.MemberID,
                    MemberName = po.Member.Name,
                    Phone = po.Member.Phone,
                    po.OrderDate,
                    po.Status,
                    po.PaymentStat,
                    po.FrontDeskNotes
                })
                .ToListAsync();

            return orders.Any() ? Ok(orders) : NoContent();
        }

        //得到某客戶所有訂單的資料 (條列式)
        [HttpGet("search/{id}")]
        public async Task<IActionResult> GetOrder(int memberId)
        {
            var orders = await _context.ProductOrder
                        .AsNoTracking()
                        .Where(p => p.MemberID == memberId)
                        .OrderByDescending(p => p.OrderDate)
                        .Select(p => new
                        {
                            p.OrderID,
                            p.MemberID,
                            p.OrderDate,
                            p.Status,
                            p.PaymentStat
                        }).ToListAsync();

            return orders.Any() ? Ok(orders) : NotFound(new { Message = "No orders found for the specified member ." });
        }

        //得到訂單明細的資料 (類似newsContent的概念 => 先點條列式的訂單,在call此api顯示明細 註:此處id指orderId)
        [HttpGet("order/{id}")]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            var orderDetail = await _context.ProductOrderItem
             .AsNoTracking()
             .Include(po => po.Product) 
             .Include(po => po.ProductOrder)
             .Where(po => po.OrderID == id)
             .Select(po => new
             {
                 po.ProductID,
                 po.Product.ProductName, 
                 po.Quantity,
                 po.TotalPrice,
             })
             .ToListAsync();

            return orderDetail.Any() ? Ok(orderDetail) : NoContent();
        }

        //新增訂單
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateOrder([FromForm] ProductOrderModel order)
        {
            var client = await _context.Member
                                       .Where(c => c.Email == order.memberEmail)
                                       .FirstOrDefaultAsync();

            if (client == null)
                return BadRequest("Invalid member email.");

            if (order.orderDate < DateTime.Today)
                return BadRequest("Invalid order dates.");

            ProductOrder po = new ProductOrder()
            {
                MemberID = client.MemberID,
                OrderDate = order.orderDate,
                Status = OrderStatus.Confirmed,
                PaymentStat = (PaymentStatus) order.paymentStatus, //有疑慮,新增的訂單基本都是Confirmed(訂單成立),看要在前端寫死或後端寫死
                FrontDeskNotes = order.frontDeskNotes,
                BackOfficeFeedback = ""
            };

            _context.ProductOrder.Add(po);
            await _context.SaveChangesAsync();

            foreach (var item in order.OrderItems)
            {
                foreach (var kvp in item)
                {
                    var productId = kvp.Key;
                    var quantity = kvp.Value;

                    ProductOrderItem poi = new ProductOrderItem()
                    {
                        OrderID = po.OrderID,      
                        ProductID = productId,      
                        Quantity = quantity,           
                        TotalPrice = (int)(quantity * _context.Product
                                           .Where(p => p.ProductID == productId)
                                           .Select(p => p.Price)
                                           .FirstOrDefault()) 
                    };

                    _context.ProductOrderItem.Add(poi);
                }
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateOrder),
                new { memberName = client.Name, phone = client.Phone }, po);
        }

        // 變更訂單狀態
        [HttpPut]
        public async Task<IActionResult> UpdateOrderStatus([FromForm] UpdateOrder request)
        {
            var order = await _context.ProductOrder
                .FirstOrDefaultAsync(po => po.OrderID == request.OrderID);

            if (order == null) return NotFound(new { Message = "No order found for the specified order id." });

            order.PaymentStat = (PaymentStatus)request.paymentStatus;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Payment status has been updated successfully." });
        }

        
        //用來更新一筆訂單中的商品明細 ex. 1杯奶茶 => 2杯奶茶,3杯綠茶 .實作方式是將該筆訂單的所有明細刪除並重新add
        [HttpPut("OrderDetail")]
        public async Task<IActionResult> UpdateOrderItem([FromForm] UpdateOrderDetail request)
        {
            var order = await _context
                                .ProductOrder
                                .Where(o => o.OrderID == request.OrderID)
                                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound("No such order.");

            // 刪除舊的訂單明細
            var existingOrderItems = await _context.ProductOrderItem
                                                   .Where(poi => poi.OrderID == request.OrderID)
                                                   .ToListAsync();

            if (existingOrderItems.Any())
            {
                _context.ProductOrderItem.RemoveRange(existingOrderItems);
                await _context.SaveChangesAsync();
            }

            // 添加新的訂單明細
            foreach (var item in request.OrderItems)
            {
                foreach (var kvp in item)
                {
                    var productId = kvp.Key;
                    var quantity = kvp.Value;

                    var product = await _context.Product
                                                     .Where(p => p.ProductID == productId)
                                                     .FirstOrDefaultAsync();

                    if (product == default)
                    {
                        return BadRequest($"Product with ID {productId} not found.");
                    }

                    var newOrderItem = new ProductOrderItem()
                    {
                        OrderID = request.OrderID,
                        ProductID = productId,
                        Quantity = quantity,
                        TotalPrice = (int)(quantity * _context.Product
                                           .Where(p => p.ProductID == productId)
                                           .Select(p => p.Price)
                                           .FirstOrDefault())
                    };

                    _context.ProductOrderItem.Add(newOrderItem);
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Order items updated successfully.");
        }

        // 根據orderID取消訂單
        [HttpDelete]
        public async Task<IActionResult> CancelBooking([FromBody] int orderId)
        {
            var order = await _context.ProductOrder
                 .Include(po => po.Member)
                 .FirstOrDefaultAsync(po => po.OrderID == orderId);

            if (order == null)
                return NotFound(new { Message = "No order found for the specified member name, phone, and order ID." });

            if (OrderStatus.Pending.Equals(order.Status))
                return BadRequest(new { Message = "Only order with status '預訂中' can be canceled." });

            if (PaymentStatus.Paid.Equals(order.PaymentStat))
                return BadRequest(new { Message = "訂單已付款，無法取消訂單" });

            order.Status = OrderStatus.Canceled;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "The order has been canceled successfully." });
        }

        public class ProductOrderModel
        {
            public string? memberEmail { get; set; }
            public DateTime orderDate { get; set; }
            public int? paymentStatus { get; set; }
            public string? frontDeskNotes { get; set; }
            public List<Dictionary<int, int>> OrderItems { get; set; } = new List<Dictionary<int, int>>(); //List of 1對1 資料結構(map) 商品id:數量
        }

        public class UpdateOrder
        {
            public int? OrderID { get; set; }
            public int? paymentStatus { get; set; }
        }

        public class UpdatePaymentRequest
        {
            public string? MemberName { get; set; }
            public string? Phone { get; set; }
            public int OrderID { get; set; }
            public string? NewPaymentStatus { get; set; }
        }

        public class UpdateOrderDetail
        {
            public int OrderID { get; set; }
            public List<Dictionary<int, int>> OrderItems { get; set; } = new List<Dictionary<int, int>>();
        }
    }
}
