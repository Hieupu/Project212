using System;
using System.Collections.Generic;

namespace Project212.Models;

public partial class Account
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Citizen> Citizens { get; set; } = new List<Citizen>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<Notice> Notices { get; set; } = new List<Notice>();

    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
