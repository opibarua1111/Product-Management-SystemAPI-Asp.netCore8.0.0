using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.IServices;
using ProductManagementAPI.Models.RequestModels;
using ProductManagementAPI.Services;
using System.Security.Claims;

namespace ProductManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("orderProduct")]
        public async Task<IActionResult> OrderProduct(ProductOrderRequest data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request Model 'saveProduct'");
                }
                if (data.Variants.Count == 0)
                {
                    throw new Exception("Add at least one variant");
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID is missing. Please login again and send valid token.");
                }
                var result = await _orderService.OrderProduct(data, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ex.InnerException is null ? BadRequest(ex.Message) : BadRequest(ex.InnerException.Message);
            }
        }
        #region Update order And must send Order IsDelete property
        [HttpPost("updateOrderProduct")]
        public async Task<IActionResult> UpdateOrderProduct(ProductOrderRequest data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request Model 'saveProduct'");
                }
                if (data.Variants.Count == 0)
                {
                    throw new Exception("Add at least one variant");
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID is missing. Please login again and send valid token.");
                }
                var result = await _orderService.UpdateOrderProduct(data, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ex.InnerException is null ? BadRequest(ex.Message) : BadRequest(ex.InnerException.Message);
            }
        }
        #endregion
        #region Specific User Can get his/her active orderList By UserId
        [HttpGet("getOrders")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID is missing. Please login again and send valid token.");
                }
                var data = await _orderService.GetOrders(userId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return ex.InnerException is null ? BadRequest(ex.Message) : BadRequest(ex.InnerException.Message);
            }
        }
        #endregion
        [HttpDelete("deleteOrderById")]
        public async Task<IActionResult> DeleteOrderById(Guid OrderId)
        {
            try
            {
                if (OrderId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(OrderId));
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _orderService.DeleteOrderById(OrderId, userId);
                return Ok("Order Deleted successfully");
            }
            catch (Exception ex)
            {
                return ex.InnerException is null ? BadRequest(ex.Message) : BadRequest(ex.InnerException.Message);
            }
        }
    }
}
