using System;
using System.Collections.Generic;

namespace SolicitatieTracker.Infrastructure.Data.Entities;

public partial class ApplicationNote
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }

    public string NoteText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Application Application { get; set; } = null!;
}
