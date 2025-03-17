using System;
using System.Collections.Generic;

namespace Project212.Models;

public partial class Record
{
    public int Id { get; set; }

    public int VehicleId { get; set; }

    public bool Result { get; set; }

    public decimal Co { get; set; }

    public decimal Hc { get; set; }

    public decimal Nox { get; set; }

    public string Note { get; set; } = null!;

    public int TimeId { get; set; }

    public int StandardId { get; set; }

    public virtual Standard Standard { get; set; } = null!;

    public virtual Timetable Time { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
