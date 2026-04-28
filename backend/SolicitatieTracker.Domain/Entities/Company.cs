using System;
using System.Collections.Generic;

namespace SollicitatieTracker.Domain.Entities;

public partial class Company
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Website { get; set; }

    public string? Location { get; set; }

    public string? Industry { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual User User { get; set; } = null!;
}
