using System;
using System.Collections.Generic;

namespace Project212.Models;

public partial class Citizen
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Address { get; set; }

    public int? Phone { get; set; }

    public string? Mail { get; set; }

    public int AccId { get; set; }

    public virtual Account Acc { get; set; } = null!;

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
