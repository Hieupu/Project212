using System;
using System.Collections.Generic;

namespace Project212.Models;

public partial class Citizen
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Address { get; set; } = null!;

    public int Phone { get; set; }

    public string Mail { get; set; } = null!;

    public int AccId { get; set; }

    public virtual Account Acc { get; set; } = null!;

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
