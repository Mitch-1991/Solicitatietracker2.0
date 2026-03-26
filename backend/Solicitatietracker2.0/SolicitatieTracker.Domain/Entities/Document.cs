using System;
using System.Collections.Generic;

namespace SolicitatieTracker.Domain.Entities;

public partial class Document
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }

    public string DocumentType { get; set; } = null!;

    public string OriginalFileName { get; set; } = null!;

    public string StoredFileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    public long FileSizeBytes { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Application Application { get; set; } = null!;
}
