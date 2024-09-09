using System;
using System.Collections.Generic;

namespace Task1.DAL.Entities;

public partial class TblProduct
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public int? Quantity { get; set; }

    public string Img { get; set; } = null!;

    public virtual ICollection<TblOrderDetail> TblOrderDetails { get; set; } = new List<TblOrderDetail>();
}
