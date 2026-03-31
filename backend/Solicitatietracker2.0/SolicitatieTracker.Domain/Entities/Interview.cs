using System;
using System.Collections.Generic;

namespace SollicitatieTracker.Domain.Entities;

public partial class Interview
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }

    public string InterviewType { get; set; } = null!;

    public DateTime ScheduledStart { get; set; }

    public DateTime? ScheduledEnd { get; set; }

    public string? Location { get; set; }

    public string? MeetingLink { get; set; }

    public string? ContactPerson { get; set; }

    public string? ContactEmail { get; set; }

    public string? Notes { get; set; }

    public string? Outcome { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Application Application { get; set; } = null!;
}
