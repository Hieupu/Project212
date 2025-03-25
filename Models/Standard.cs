using System;
using System.Collections.Generic;

namespace Project212.Models;

public partial class Standard
{
    public int Id { get; set; }

    public double Co { get; set; }

    public double Hc { get; set; }

    public double Nox { get; set; }

    public DateOnly Date { get; set; }

    public virtual ICollection<Record> Records { get; set; } = new List<Record>();
}
