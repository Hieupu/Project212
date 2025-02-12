using System;
using System.Collections.Generic;

namespace Project212.Models;

public partial class Notice
{
    public int Id { get; set; }

    public int AccId { get; set; }

    public string Detail { get; set; } = null!;

    public DateTime SentDate { get; set; }

    public bool IsRead { get; set; }

    public virtual Account Acc { get; set; } = null!;
}
