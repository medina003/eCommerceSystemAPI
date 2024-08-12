using eCommerceRESTfulAPI.Application.DTOs;
using eCommerceRESTfulAPI.Application.Interfaces.Services;
using eCommerceRESTfulAPI.Domain.Entities;
using eCommerceRESTfulAPI.Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceRESTfulAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            var order = new Order
            {
                CustomerId = orderCreateDto.CustomerId,
                OrderItems = orderCreateDto.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            foreach (var item in order.OrderItems)
            {
                var product = await _orderService.GetProductByIdAsync(item.ProductId);
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    return BadRequest($"Insufficient stock for product ID {item.ProductId}.");
                }
            }

            await _orderService.CreateAsync(order);

            foreach (var item in order.OrderItems)
            {
                await _orderService.DecreaseStockAsync(item.ProductId, item.Quantity);
            }

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Items = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList(),
                TotalPrice = order.TotalPrice,
                Status = order.Status.ToString()
            };

            return Ok(orderDto);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var existingOrder = await _orderService.GetByIdAsync(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            existingOrder.Status = OrderStatus.Cancelled;

            foreach (var item in existingOrder.OrderItems)
            {
                await _orderService.RestockAsync(item.ProductId, item.Quantity);
            }

            await _orderService.UpdateAsync(existingOrder);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, [FromBody] OrderDto orderDto)
        {
            var existingOrder = await _orderService.GetByIdAsync(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            existingOrder.CustomerId = orderDto.CustomerId;
            existingOrder.OrderItems = orderDto.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList();

            if (Enum.TryParse<OrderStatus>(orderDto.Status, true, out var status))
            {
                existingOrder.Status = status;
            }
            else
            {
                return BadRequest("Invalid order status.");
            }

            await _orderService.UpdateAsync(existingOrder);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteAsync(id);
            return NoContent();
        }
    }
}
