using System;
using System.Collections.Generic;

namespace WpfApp4.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string Address { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
