using System;
using System.Collections.Generic;

namespace SollicitatieTracker.Domain.Entities;

public partial class Application
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CompanyId { get; set; }

    public string JobTitle { get; set; } = null!;

    public string? JobUrl { get; set; }

    public Status Status { get; set; }

    public string? Priority { get; set; }

    public DateOnly? AppliedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public string? NextStep { get; set; }

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public string? Source { get; set; }

    public bool IsArchived { get; set; }

    public DateTime? ArchivedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ApplicationNote> ApplicationNotes { get; set; } = new List<ApplicationNote>();

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual ICollection<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual User User { get; set; } = null!;
}
