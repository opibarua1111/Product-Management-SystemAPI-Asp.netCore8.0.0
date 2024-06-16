using ProductManagementAPI.Models;
using ProductManagementAPI.Models.RequestModels;
using ProductManagementAPI.Models.ResponseModels;

namespace ProductManagementAPI.IServices
{
    public interface IProductService
    {
        Task<CustomResponse> GetProductList(ListRequestModel model);
        Task<CustomResponse> GetProductById(Guid Id);
        Task SaveProduct(ProductRequestModel data, string userId);
        Task DeleteProductById(Guid Id, string userId);
    }
}
