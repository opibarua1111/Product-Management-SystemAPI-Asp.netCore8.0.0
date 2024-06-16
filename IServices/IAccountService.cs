using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Models.RequestModels;
using ProductManagementAPI.Models.ResponseModels;

namespace ProductManagementAPI.IServices
{
    public interface IAccountService
    {
        public Task<RegistrationResponse> Register(RegisterModel model);
        public Task<LoginResponse> Login(LoginModel model);
    }
}
