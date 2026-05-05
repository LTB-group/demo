using System;
using System.Collections.Generic;

namespace WpfApp4.Models;

public partial class Product
{
    public string ProductCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public int DiscountAmount { get; set; }

    public int LocationId { get; set; }

    public string? Description { get; set; }

    public string? PhotoPath { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual Manufacturer Manufacturer { get; set; } = null!;
}
