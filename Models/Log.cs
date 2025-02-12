using System;
using System.Collections.Generic;

namespace Project212.Models;

public partial class Log
{
    public int Id { get; set; }

    public int AccId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual Account Acc { get; set; } = null!;
}
