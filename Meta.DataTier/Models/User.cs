using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public string? Status { get; set; }
}
