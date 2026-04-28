using Microsoft.EntityFrameworkCore;
using SollicitatieTracker.Infrastructure.Data;
using TaskSystem = System.Threading.Tasks.Task;

namespace Sollicitatietracker_API.Data;

public static class DatabaseInitializer
{
    public static async TaskSystem InitializeAsync(SollicitatietrackerDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        await EnsureAuthColumnsAsync(context);
        await EnsureApplicationArchiveColumnsAsync(context);
        await EnsureEmailOutboxTableAsync(context);
    }

    private static async TaskSystem EnsureAuthColumnsAsync(SollicitatietrackerDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("""
            IF COL_LENGTH('users', 'password_reset_token_hash') IS NULL
            BEGIN
                ALTER TABLE users ADD password_reset_token_hash nvarchar(500) NULL;
            END

            IF COL_LENGTH('users', 'password_reset_token_expires_at') IS NULL
            BEGIN
                ALTER TABLE users ADD password_reset_token_expires_at datetime2 NULL;
            END
            """);
    }

    private static async TaskSystem EnsureApplicationArchiveColumnsAsync(SollicitatietrackerDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("""
            IF COL_LENGTH('applications', 'is_archived') IS NULL
            BEGIN
                ALTER TABLE applications ADD is_archived bit NOT NULL CONSTRAINT DF_applications_is_archived DEFAULT 0;
            END

            IF COL_LENGTH('applications', 'archived_at') IS NULL
            BEGIN
                ALTER TABLE applications ADD archived_at datetime2 NULL;
            END
            """);
    }

    private static async TaskSystem EnsureEmailOutboxTableAsync(SollicitatietrackerDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID('email_outbox_messages', 'U') IS NULL
            BEGIN
                CREATE TABLE email_outbox_messages
                (
                    id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_email_outbox_messages PRIMARY KEY,
                    to_email nvarchar(255) NOT NULL,
                    subject nvarchar(255) NOT NULL,
                    html_body nvarchar(max) NOT NULL,
                    text_body nvarchar(max) NOT NULL,
                    status nvarchar(30) NOT NULL CONSTRAINT DF_email_outbox_messages_status DEFAULT 'Pending',
                    attempts int NOT NULL CONSTRAINT DF_email_outbox_messages_attempts DEFAULT 0,
                    next_attempt_at datetime2 NULL CONSTRAINT DF_email_outbox_messages_next_attempt_at DEFAULT sysutcdatetime(),
                    last_error nvarchar(max) NULL,
                    created_at datetime2 NOT NULL CONSTRAINT DF_email_outbox_messages_created_at DEFAULT sysutcdatetime(),
                    sent_at datetime2 NULL
                );

                CREATE INDEX IX_email_outbox_messages_status_next_attempt_at
                    ON email_outbox_messages(status, next_attempt_at);
            END
            """);
    }
}
