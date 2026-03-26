using System;
using System.Collections.Generic;

namespace SolicitatieTracker.Domain.Entities;

public partial class StatusHistory
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }

    public Status? OldStatus { get; set; }

    public Status NewStatus { get; set; } 

    public DateTime ChangedAt { get; set; }

    public virtual Application Application { get; set; } = null!;
}
