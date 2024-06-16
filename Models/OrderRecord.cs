using System;
using System.Collections.Generic;

namespace ProductManagementAPI.Models;

public partial class OrderRecord
{
    public Guid OrderRecordId { get; set; }

    public Guid OrderId { get; set; }

    public Guid VariantId { get; set; }

    public Guid CreatedById { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? DeletedById { get; set; }

    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? ModifiedById { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Variant Variant { get; set; } = null!;
}
