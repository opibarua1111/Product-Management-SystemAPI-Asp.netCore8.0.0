using ProductManagementAPI.Models.RequestModels;
using ProductManagementAPI.Models.ResponseModels;

namespace ProductManagementAPI.IServices
{
    public interface IOrderService
    {
        Task<CustomResponse> OrderProduct(ProductOrderRequest data, string userId);
        Task<CustomResponse> UpdateOrderProduct(ProductOrderRequest data, string userId);
        Task<CustomResponse> GetOrders(string userId);
        Task DeleteOrderById(Guid OrderId, string userId);
    }
}
