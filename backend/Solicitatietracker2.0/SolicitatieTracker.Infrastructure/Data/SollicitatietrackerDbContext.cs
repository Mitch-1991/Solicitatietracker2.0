using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SolicitatieTracker.Domain.Entities;
using TaskEntity = SolicitatieTracker.Domain.Entities.Task;

namespace SolicitatieTracker.Infrastructure.Data;

public partial class SollicitatietrackerDbContext : DbContext
{
    public SollicitatietrackerDbContext(DbContextOptions<SollicitatietrackerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<ApplicationNote> ApplicationNotes { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Interview> Interviews { get; set; }

    public virtual DbSet<StatusHistory> StatusHistories { get; set; }

    public virtual DbSet<TaskEntity> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__applicat__3213E83F55704D96");

            entity.ToTable("applications");

            entity.HasIndex(e => e.CompanyId, "IX_applications_company_id");

            entity.HasIndex(e => e.Status, "IX_applications_status");

            entity.HasIndex(e => e.UserId, "IX_applications_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppliedDate).HasColumnName("applied_date");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(200)
                .HasColumnName("job_title");
            entity.Property(e => e.JobUrl)
                .HasMaxLength(1000)
                .HasColumnName("job_url");
            entity.Property(e => e.NextStep)
                .HasMaxLength(255)
                .HasColumnName("next_step");
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .HasColumnName("priority");
            entity.Property(e => e.SalaryMax)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("salary_max");
            entity.Property(e => e.SalaryMin)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("salary_min");
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .HasColumnName("source");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Company).WithMany(p => p.Applications)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_applications_companies");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_applications_users");
        });

        modelBuilder.Entity<ApplicationNote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__applicat__3213E83F6AB22BBE");

            entity.ToTable("application_notes");

            entity.HasIndex(e => e.ApplicationId, "IX_application_notes_application_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.NoteText).HasColumnName("note_text");

            entity.HasOne(d => d.Application).WithMany(p => p.ApplicationNotes)
                .HasForeignKey(d => d.ApplicationId)
                .HasConstraintName("FK_application_notes_applications");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__companie__3213E83F60DD0676");

            entity.ToTable("companies");

            entity.HasIndex(e => e.UserId, "IX_companies_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Industry)
                .HasMaxLength(150)
                .HasColumnName("industry");
            entity.Property(e => e.Location)
                .HasMaxLength(150)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("website");

            entity.HasOne(d => d.User).WithMany(p => p.Companies)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_companies_users");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__document__3213E83F758C73C5");

            entity.ToTable("documents");

            entity.HasIndex(e => e.ApplicationId, "IX_documents_application_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.DocumentType)
                .HasMaxLength(50)
                .HasColumnName("document_type");
            entity.Property(e => e.FilePath)
                .HasMaxLength(1000)
                .HasColumnName("file_path");
            entity.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes");
            entity.Property(e => e.MimeType)
                .HasMaxLength(100)
                .HasColumnName("mime_type");
            entity.Property(e => e.OriginalFileName)
                .HasMaxLength(255)
                .HasColumnName("original_file_name");
            entity.Property(e => e.StoredFileName)
                .HasMaxLength(255)
                .HasColumnName("stored_file_name");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("uploaded_at");

            entity.HasOne(d => d.Application).WithMany(p => p.Documents)
                .HasForeignKey(d => d.ApplicationId)
                .HasConstraintName("FK_documents_applications");
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__intervie__3213E83F597818B6");

            entity.ToTable("interviews");

            entity.HasIndex(e => e.ApplicationId, "IX_interviews_application_id");

            entity.HasIndex(e => e.ScheduledStart, "IX_interviews_scheduled_start");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(255)
                .HasColumnName("contact_email");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(150)
                .HasColumnName("contact_person");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.InterviewType)
                .HasMaxLength(50)
                .HasColumnName("interview_type");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.MeetingLink)
                .HasMaxLength(1000)
                .HasColumnName("meeting_link");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Outcome)
                .HasMaxLength(50)
                .HasColumnName("outcome");
            entity.Property(e => e.ScheduledEnd).HasColumnName("scheduled_end");
            entity.Property(e => e.ScheduledStart).HasColumnName("scheduled_start");

            entity.HasOne(d => d.Application).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.ApplicationId)
                .HasConstraintName("FK_interviews_applications");
        });

        modelBuilder.Entity<StatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__status_h__3213E83FF7291446");

            entity.ToTable("status_history");

            entity.HasIndex(e => e.ApplicationId, "IX_status_history_application_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("changed_at");
            entity.Property(e => e.NewStatus)
                .HasMaxLength(50)
                .HasColumnName("new_status");
            entity.Property(e => e.OldStatus)
                .HasMaxLength(50)
                .HasColumnName("old_status");

            entity.HasOne(d => d.Application).WithMany(p => p.StatusHistories)
                .HasForeignKey(d => d.ApplicationId)
                .HasConstraintName("FK_status_history_applications");
        });

        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tasks__3213E83F07A4ECBB");

            entity.ToTable("tasks");

            entity.HasIndex(e => e.ApplicationId, "IX_tasks_application_id");

            entity.HasIndex(e => e.UserId, "IX_tasks_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.IsCompleted).HasColumnName("is_completed");
            entity.Property(e => e.TaskType)
                .HasMaxLength(50)
                .HasColumnName("task_type");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Application).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_tasks_applications");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F17E204F1");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164A33FBD4A").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .HasColumnName("password_hash");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
