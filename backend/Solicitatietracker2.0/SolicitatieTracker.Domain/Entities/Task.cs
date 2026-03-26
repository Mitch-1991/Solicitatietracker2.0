using System;
using System.Collections.Generic;

namespace SolicitatieTracker.Domain.Entities;

public partial class Task
{
    public int Id { get; set; }

    public int? ApplicationId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public string? TaskType { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Application? Application { get; set; }
}
