using System;
using System.Collections.Generic;

namespace Project212.Models;

public partial class InspectionStation
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int Phone { get; set; }

    public string Mail { get; set; } = null!;

    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
