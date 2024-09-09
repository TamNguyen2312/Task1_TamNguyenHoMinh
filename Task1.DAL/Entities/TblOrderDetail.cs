using System;
using System.Collections.Generic;

namespace Task1.DAL.Entities;

public partial class TblOrderDetail
{
    public int Oid { get; set; }

    public string Pid { get; set; } = null!;

    public int Quantity { get; set; }

    public double Price { get; set; }

    public virtual TblOrder OidNavigation { get; set; } = null!;

    public virtual TblProduct PidNavigation { get; set; } = null!;
}
