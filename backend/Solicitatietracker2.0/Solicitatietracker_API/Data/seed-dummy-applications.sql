SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS (
        SELECT 1
        FROM users
        WHERE email = 'dummy.user@sollicitatietracker.local'
    )
    BEGIN
        INSERT INTO users (first_name, last_name, email, password_hash, created_at)
        VALUES ('Demo', 'Gebruiker', 'dummy.user@sollicitatietracker.local', 'seeded-dummy-password-hash', '2026-02-01T09:00:00');
    END;

    DECLARE @UserId INT = (
        SELECT id
        FROM users
        WHERE email = 'dummy.user@sollicitatietracker.local'
    );

    IF NOT EXISTS (SELECT 1 FROM companies WHERE user_id = @UserId AND name = 'TechNova')
        INSERT INTO companies (user_id, name, website, location, industry, notes, created_at)
        VALUES (@UserId, 'TechNova', 'https://technova.example', 'Brussel', 'Software', 'Focus op SaaS-oplossingen.', '2026-02-02T09:00:00');

    IF NOT EXISTS (SELECT 1 FROM companies WHERE user_id = @UserId AND name = 'DataPulse')
        INSERT INTO companies (user_id, name, website, location, industry, notes, created_at)
        VALUES (@UserId, 'DataPulse', 'https://datapulse.example', 'Antwerpen', 'Data & AI', 'Zoekt full stack profielen.', '2026-02-04T09:00:00');

    IF NOT EXISTS (SELECT 1 FROM companies WHERE user_id = @UserId AND name = 'CloudPeak')
        INSERT INTO companies (user_id, name, website, location, industry, notes, created_at)
        VALUES (@UserId, 'CloudPeak', 'https://cloudpeak.example', 'Gent', 'Cloud', 'Sterke Azure-omgeving.', '2026-02-06T09:00:00');

    IF NOT EXISTS (SELECT 1 FROM companies WHERE user_id = @UserId AND name = 'PixelForge')
        INSERT INTO companies (user_id, name, website, location, industry, notes, created_at)
        VALUES (@UserId, 'PixelForge', 'https://pixelforge.example', 'Leuven', 'Product Design', 'Frontend heavy team.', '2026-02-08T09:00:00');

    IF NOT EXISTS (SELECT 1 FROM companies WHERE user_id = @UserId AND name = 'SecureLayer')
        INSERT INTO companies (user_id, name, website, location, industry, notes, created_at)
        VALUES (@UserId, 'SecureLayer', 'https://securelayer.example', 'Mechelen', 'Cybersecurity', 'Backend rol met security-focus.', '2026-02-10T09:00:00');

    IF NOT EXISTS (SELECT 1 FROM companies WHERE user_id = @UserId AND name = 'GreenGrid')
        INSERT INTO companies (user_id, name, website, location, industry, notes, created_at)
        VALUES (@UserId, 'GreenGrid', 'https://greengrid.example', 'Hasselt', 'EnergyTech', 'Werkt met IoT-platformen.', '2026-02-12T09:00:00');

    IF NOT EXISTS (SELECT 1 FROM companies WHERE user_id = @UserId AND name = 'FinCore')
        INSERT INTO companies (user_id, name, website, location, industry, notes, created_at)
        VALUES (@UserId, 'FinCore', 'https://fincore.example', 'Brugge', 'FinTech', 'C# en React stack.', '2026-02-14T09:00:00');

    IF NOT EXISTS (SELECT 1 FROM companies WHERE user_id = @UserId AND name = 'HealthBridge')
        INSERT INTO companies (user_id, name, website, location, industry, notes, created_at)
        VALUES (@UserId, 'HealthBridge', 'https://healthbridge.example', 'Kortrijk', 'HealthTech', 'Zoekt API-ervaring.', '2026-02-16T09:00:00');

    IF NOT EXISTS (SELECT 1 FROM companies WHERE user_id = @UserId AND name = 'LogiFlow')
        INSERT INTO companies (user_id, name, website, location, industry, notes, created_at)
        VALUES (@UserId, 'LogiFlow', 'https://logiflow.example', 'Genk', 'Logistics', 'Scrum team, hybride.', '2026-02-18T09:00:00');

    IF NOT EXISTS (SELECT 1 FROM companies WHERE user_id = @UserId AND name = 'BrightApps')
        INSERT INTO companies (user_id, name, website, location, industry, notes, created_at)
        VALUES (@UserId, 'BrightApps', 'https://brightapps.example', 'Namen', 'Consulting', 'Projectconsultancy.', '2026-02-20T09:00:00');

    DECLARE @TechNovaId INT = (SELECT id FROM companies WHERE user_id = @UserId AND name = 'TechNova');
    DECLARE @DataPulseId INT = (SELECT id FROM companies WHERE user_id = @UserId AND name = 'DataPulse');
    DECLARE @CloudPeakId INT = (SELECT id FROM companies WHERE user_id = @UserId AND name = 'CloudPeak');
    DECLARE @PixelForgeId INT = (SELECT id FROM companies WHERE user_id = @UserId AND name = 'PixelForge');
    DECLARE @SecureLayerId INT = (SELECT id FROM companies WHERE user_id = @UserId AND name = 'SecureLayer');
    DECLARE @GreenGridId INT = (SELECT id FROM companies WHERE user_id = @UserId AND name = 'GreenGrid');
    DECLARE @FinCoreId INT = (SELECT id FROM companies WHERE user_id = @UserId AND name = 'FinCore');
    DECLARE @HealthBridgeId INT = (SELECT id FROM companies WHERE user_id = @UserId AND name = 'HealthBridge');
    DECLARE @LogiFlowId INT = (SELECT id FROM companies WHERE user_id = @UserId AND name = 'LogiFlow');
    DECLARE @BrightAppsId INT = (SELECT id FROM companies WHERE user_id = @UserId AND name = 'BrightApps');

    IF NOT EXISTS (SELECT 1 FROM applications WHERE user_id = @UserId AND company_id = @TechNovaId AND job_title = 'Junior .NET Developer')
        INSERT INTO applications (user_id, company_id, job_title, job_url, status, priority, applied_date, next_step, salary_min, salary_max, source, created_at, updated_at)
        VALUES (@UserId, @TechNovaId, 'Junior .NET Developer', 'https://technova.example/jobs/junior-dotnet', 'Verzonden', 'High', '2026-02-05', 'Wachten op eerste feedback', 2800.00, 3400.00, 'LinkedIn', '2026-02-05T10:00:00', '2026-02-05T10:00:00');

    IF NOT EXISTS (SELECT 1 FROM applications WHERE user_id = @UserId AND company_id = @DataPulseId AND job_title = 'Full Stack Developer')
        INSERT INTO applications (user_id, company_id, job_title, job_url, status, priority, applied_date, next_step, salary_min, salary_max, source, created_at, updated_at)
        VALUES (@UserId, @DataPulseId, 'Full Stack Developer', 'https://datapulse.example/jobs/full-stack', 'Gesprek', 'High', '2026-02-09', 'Technisch interview', 3200.00, 3800.00, 'VDAB', '2026-02-09T11:00:00', '2026-02-18T09:00:00');

    IF NOT EXISTS (SELECT 1 FROM applications WHERE user_id = @UserId AND company_id = @CloudPeakId AND job_title = 'Cloud Engineer')
        INSERT INTO applications (user_id, company_id, job_title, job_url, status, priority, applied_date, next_step, salary_min, salary_max, source, created_at, updated_at)
        VALUES (@UserId, @CloudPeakId, 'Cloud Engineer', 'https://cloudpeak.example/jobs/cloud-engineer', 'Afgewezen', 'Medium', '2026-02-13', 'Afgesloten', 3400.00, 4100.00, 'Indeed', '2026-02-13T09:30:00', '2026-02-21T14:00:00');

    IF NOT EXISTS (SELECT 1 FROM applications WHERE user_id = @UserId AND company_id = @PixelForgeId AND job_title = 'React Frontend Developer')
        INSERT INTO applications (user_id, company_id, job_title, job_url, status, priority, applied_date, next_step, salary_min, salary_max, source, created_at, updated_at)
        VALUES (@UserId, @PixelForgeId, 'React Frontend Developer', 'https://pixelforge.example/jobs/react-frontend', 'Verzonden', 'Low', '2026-02-17', 'Portfolio opvolgen', 2900.00, 3500.00, 'LinkedIn', '2026-02-17T08:45:00', '2026-02-17T08:45:00');

    IF NOT EXISTS (SELECT 1 FROM applications WHERE user_id = @UserId AND company_id = @SecureLayerId AND job_title = 'Backend Developer')
        INSERT INTO applications (user_id, company_id, job_title, job_url, status, priority, applied_date, next_step, salary_min, salary_max, source, created_at, updated_at)
        VALUES (@UserId, @SecureLayerId, 'Backend Developer', 'https://securelayer.example/jobs/backend', 'Gesprek', 'High', '2026-02-22', 'Case voorbereiden', 3300.00, 4000.00, 'StepStone', '2026-02-22T13:15:00', '2026-03-03T10:00:00');

    IF NOT EXISTS (SELECT 1 FROM applications WHERE user_id = @UserId AND company_id = @GreenGridId AND job_title = 'Software Engineer')
        INSERT INTO applications (user_id, company_id, job_title, job_url, status, priority, applied_date, next_step, salary_min, salary_max, source, created_at, updated_at)
        VALUES (@UserId, @GreenGridId, 'Software Engineer', 'https://greengrid.example/jobs/software-engineer', 'Verzonden', 'Medium', '2026-02-27', 'Recruiter mail afwachten', 3000.00, 3600.00, 'Company Site', '2026-02-27T09:00:00', '2026-02-27T09:00:00');

    IF NOT EXISTS (SELECT 1 FROM applications WHERE user_id = @UserId AND company_id = @FinCoreId AND job_title = 'C# Developer')
        INSERT INTO applications (user_id, company_id, job_title, job_url, status, priority, applied_date, next_step, salary_min, salary_max, source, created_at, updated_at)
        VALUES (@UserId, @FinCoreId, 'C# Developer', 'https://fincore.example/jobs/csharp', 'Aanbieding', 'High', '2026-03-03', 'Aanbod evalueren', 3600.00, 4300.00, 'Referral', '2026-03-03T12:00:00', '2026-03-19T16:00:00');

    IF NOT EXISTS (SELECT 1 FROM applications WHERE user_id = @UserId AND company_id = @HealthBridgeId AND job_title = 'API Developer')
        INSERT INTO applications (user_id, company_id, job_title, job_url, status, priority, applied_date, next_step, salary_min, salary_max, source, created_at, updated_at)
        VALUES (@UserId, @HealthBridgeId, 'API Developer', 'https://healthbridge.example/jobs/api-developer', 'Afgewezen', 'Medium', '2026-03-08', 'Afgesloten', 3100.00, 3700.00, 'LinkedIn', '2026-03-08T10:30:00', '2026-03-20T15:00:00');

    IF NOT EXISTS (SELECT 1 FROM applications WHERE user_id = @UserId AND company_id = @LogiFlowId AND job_title = 'Medior Software Developer')
        INSERT INTO applications (user_id, company_id, job_title, job_url, status, priority, applied_date, next_step, salary_min, salary_max, source, created_at, updated_at)
        VALUES (@UserId, @LogiFlowId, 'Medior Software Developer', 'https://logiflow.example/jobs/medior-software', 'Verzonden', 'Medium', '2026-03-14', 'Telefonische screening', 3200.00, 3900.00, 'Indeed', '2026-03-14T09:15:00', '2026-03-14T09:15:00');

    IF NOT EXISTS (SELECT 1 FROM applications WHERE user_id = @UserId AND company_id = @BrightAppsId AND job_title = 'Consultant Developer')
        INSERT INTO applications (user_id, company_id, job_title, job_url, status, priority, applied_date, next_step, salary_min, salary_max, source, created_at, updated_at)
        VALUES (@UserId, @BrightAppsId, 'Consultant Developer', 'https://brightapps.example/jobs/consultant-developer', 'Gesprek', 'Low', '2026-03-21', 'Tweede gesprek', 3000.00, 3650.00, 'StepStone', '2026-03-21T11:30:00', '2026-03-25T09:30:00');

    DECLARE @DataPulseApplicationId INT = (
        SELECT id
        FROM applications
        WHERE user_id = @UserId AND company_id = @DataPulseId AND job_title = 'Full Stack Developer'
    );

    DECLARE @SecureLayerApplicationId INT = (
        SELECT id
        FROM applications
        WHERE user_id = @UserId AND company_id = @SecureLayerId AND job_title = 'Backend Developer'
    );

    DECLARE @BrightAppsApplicationId INT = (
        SELECT id
        FROM applications
        WHERE user_id = @UserId AND company_id = @BrightAppsId AND job_title = 'Consultant Developer'
    );

    IF NOT EXISTS (SELECT 1 FROM interviews WHERE application_id = @DataPulseApplicationId AND interview_type = 'HR')
        INSERT INTO interviews (application_id, interview_type, scheduled_start, scheduled_end, location, meeting_link, contact_person, contact_email, notes, outcome, created_at)
        VALUES (@DataPulseApplicationId, 'HR', '2026-03-28T10:00:00', '2026-03-28T11:00:00', 'Antwerpen', NULL, 'Sofie Janssens', 'sofie@datapulse.example', 'Introgesprek met HR.', NULL, '2026-03-25T10:00:00');

    IF NOT EXISTS (SELECT 1 FROM interviews WHERE application_id = @SecureLayerApplicationId AND interview_type = 'Technisch')
        INSERT INTO interviews (application_id, interview_type, scheduled_start, scheduled_end, location, meeting_link, contact_person, contact_email, notes, outcome, created_at)
        VALUES (@SecureLayerApplicationId, 'Technisch', '2026-03-30T14:00:00', '2026-03-30T15:00:00', NULL, 'https://meet.securelayer.example/backend', 'Tom Peeters', 'tom@securelayer.example', 'Technische case bespreken.', NULL, '2026-03-25T14:00:00');

    IF NOT EXISTS (SELECT 1 FROM interviews WHERE application_id = @BrightAppsApplicationId AND interview_type = 'Team Fit')
        INSERT INTO interviews (application_id, interview_type, scheduled_start, scheduled_end, location, meeting_link, contact_person, contact_email, notes, outcome, created_at)
        VALUES (@BrightAppsApplicationId, 'Team Fit', '2026-04-02T09:00:00', '2026-04-02T10:00:00', NULL, 'https://meet.brightapps.example/teamfit', 'Annelies Martin', 'annelies@brightapps.example', 'Gesprek met delivery manager.', NULL, '2026-03-26T09:00:00');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
