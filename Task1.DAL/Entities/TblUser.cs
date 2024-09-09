using System;
using System.Collections.Generic;

namespace Task1.DAL.Entities;

public partial class TblUser
{
    public string UserId { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Password { get; set; }

    public string? RoleId { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();
}
