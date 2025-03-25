using System;
using System.Collections.Generic;

namespace Project212.Models;

public partial class Timetable
{
    public int Id { get; set; }

    public int InspectionId { get; set; }

    public int AccId { get; set; }

    public DateTime InspectTime { get; set; }

    public string Status { get; set; } = null!;

    public virtual Account Acc { get; set; } = null!;

    public virtual InspectionStation Inspection { get; set; } = null!;

    public virtual ICollection<Record> Records { get; set; } = new List<Record>();

    public int? VehicleId { get; set; }

    public virtual Vehicle? Vehicle { get; set; }
}
