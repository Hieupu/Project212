using System;
using System.Collections.Generic;

namespace Project212.Models;

public partial class Vehicle
{
    public int Id { get; set; }

    public string Brand { get; set; } = null!;

    public string Color { get; set; } = null!;

    public string Engine { get; set; } = null!;

    public string Chassis { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string Plate { get; set; } = null!;

    public DateOnly Dom { get; set; }

    public int CitizenId { get; set; }

    public int? Capacity { get; set; }

    public virtual Citizen Citizen { get; set; } = null!;

    public virtual ICollection<Record> Records { get; set; } = new List<Record>();
}
