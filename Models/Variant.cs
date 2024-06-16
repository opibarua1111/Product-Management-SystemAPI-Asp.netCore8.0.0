using System;
using System.Collections.Generic;

namespace ProductManagementAPI.Models;

public partial class Variant
{
    public Guid VariantId { get; set; }

    public string Color { get; set; } = null!;

    public string Specification { get; set; } = null!;

    public string Size { get; set; } = null!;

    public Guid ProductId { get; set; }

    public string? Price { get; set; }

    public string? Image { get; set; }

    public string? Stock { get; set; }

    public string? Description { get; set; }

    public bool? IsApproved { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public bool IsDeleted { get; set; }

    public Guid CreatedById { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifiedById { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public Guid? DeletedById { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<OrderRecord> OrderRecords { get; set; } = new List<OrderRecord>();

    public virtual Product Product { get; set; } = null!;
}
