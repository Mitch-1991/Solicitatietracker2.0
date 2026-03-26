using System;
using System.Collections.Generic;

namespace SolicitatieTracker.Infrastructure.Data.Entities;

public partial class StatusHistory
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }

    public string? OldStatus { get; set; }

    public string NewStatus { get; set; } = null!;

    public DateTime ChangedAt { get; set; }

    public virtual Application Application { get; set; } = null!;
}
