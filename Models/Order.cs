using System;
using System.Collections.Generic;

namespace ProductManagementAPI.Models;

public partial class Order
{
    public Guid OrderId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Status { get; set; } = null!;

    public Guid UserId { get; set; }

    public string? Description { get; set; }

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
}
