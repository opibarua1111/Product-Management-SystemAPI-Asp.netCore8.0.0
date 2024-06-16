using Microsoft.AspNetCore.Identity;

namespace ProductManagementAPI.Models.ResponseModels
{
    public class RegistrationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }
    }
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
