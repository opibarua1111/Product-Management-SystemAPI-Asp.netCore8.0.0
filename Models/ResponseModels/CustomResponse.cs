namespace ProductManagementAPI.Models.ResponseModels
{
    public class CustomResponse
    {
        public dynamic data { get; set; }
        public dynamic Message { get; set; }
        public int ReponseCode { get; set; }
        public bool Success { get; set; }
    }
}
