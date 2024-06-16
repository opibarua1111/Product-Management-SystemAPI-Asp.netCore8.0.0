namespace ProductManagementAPI.Models.RequestModels
{
    public class ListRequestModel
    {
        public int page { get; set; }
        public int length { get; set; }
        public string searchValue { get; set; }
    }
}
