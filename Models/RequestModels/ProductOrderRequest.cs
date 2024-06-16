namespace ProductManagementAPI.Models.RequestModels
{
    public class ProductOrderRequest
    {
        public Guid OrderId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Status { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<VariantReq> Variants { get; set; } = new List<VariantReq>();
    }
    public partial class VariantReq
    {
        public Guid VariantId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
