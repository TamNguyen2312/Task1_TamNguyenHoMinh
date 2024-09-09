using System;
using System.Collections.Generic;

namespace Task1.DAL.Entities;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public int? Type { get; set; }

    public int? CustomerId { get; set; }
}
