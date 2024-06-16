using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.IServices;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.RequestModels;
using System.Security.Claims;

namespace ProductManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("getProductList")]
        public async Task<IActionResult> GetProductList(ListRequestModel model)
        {
            try
            {
                var data = await _productService.GetProductList(model);
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                return ex.InnerException is null ? BadRequest(ex.Message) : BadRequest(ex.InnerException.Message);
            }
        }
        [HttpGet("getProductById")]
        public async Task<IActionResult> GetProductById(Guid Id)
        {
            try
            {
                var data = await _productService.GetProductById(Id);
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                return ex.InnerException is null ? BadRequest(ex.Message) : BadRequest(ex.InnerException.Message);
            }
        }
        #region Product Create and Update
        [HttpPost("saveProduct")]
        public async Task<IActionResult> SaveProduct(ProductRequestModel data) 
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
                await _productService.SaveProduct(data, userId);
                return Ok("Product save Successfully!");
            }
            catch (Exception ex)
            {
                return ex.InnerException is null ? BadRequest(ex.Message) : BadRequest(ex.InnerException.Message);
            }
        }
        #endregion
        [HttpDelete("deleteProductById")]
        public async Task<IActionResult> DeleteProductById(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(Id));
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _productService.DeleteProductById(Id, userId);
                return Ok("Product Deleted successfully");
            }
            catch (Exception ex)
            {
                return ex.InnerException is null ? BadRequest(ex.Message) : BadRequest(ex.InnerException.Message);
            }
        }
    }
}
