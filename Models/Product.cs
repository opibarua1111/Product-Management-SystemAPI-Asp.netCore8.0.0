using System;
using System.Collections.Generic;

namespace ProductManagementAPI.Models;

public partial class Product
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Price { get; set; } = null!;

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

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
