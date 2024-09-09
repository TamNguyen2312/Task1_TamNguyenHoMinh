using System;
using System.Collections.Generic;

namespace Task1.DAL.Entities;

public partial class TblOrder
{
    public int Id { get; set; }

    public string? Date { get; set; }

    public string Uid { get; set; } = null!;

    public double Total { get; set; }

    public string UserName { get; set; } = null!;

    public string? ShippingAddress { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual ICollection<TblOrderDetail> TblOrderDetails { get; set; } = new List<TblOrderDetail>();

    public virtual TblUser UidNavigation { get; set; } = null!;
}
