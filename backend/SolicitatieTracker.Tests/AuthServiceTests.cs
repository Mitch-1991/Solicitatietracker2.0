using SolicitatieTracker.App.DTOs.Auth;
using SolicitatieTracker.App.Services.Auth;
using SolicitatieTracker.Infrastructure.Data.Repos.Auth;
using SolicitatieTracker.Infrastructure.Messaging;
using SollicitatieTracker.Domain.Entities;
using TaskSystem = System.Threading.Tasks.Task;

namespace SollicitatieTracker.Tests;

public class AuthServiceTests
{
    [Fact]
    public async TaskSystem LoginAsync_UsesShortExpirationWhenRememberMeIsFalse()
    {
        var user = CreateUser(password: "secret123");
        var jwtTokenService = new FakeJwtTokenService();
        var service = CreateService(new FakeUserRepository(user), jwtTokenService);

        var result = await service.LoginAsync(new LoginRequestDto
        {
            Email = user.Email,
            Password = "secret123",
            RememberMe = false
        });

        Assert.False(jwtTokenService.LastRememberMe);
        Assert.Equal(jwtTokenService.ShortExpiration, result.ExpiresAt);
    }

    [Fact]
    public async TaskSystem LoginAsync_UsesLongExpirationWhenRememberMeIsTrue()
    {
        var user = CreateUser(password: "secret123");
        var jwtTokenService = new FakeJwtTokenService();
        var service = CreateService(new FakeUserRepository(user), jwtTokenService);

        var result = await service.LoginAsync(new LoginRequestDto
        {
            Email = user.Email,
            Password = "secret123",
            RememberMe = true
        });

        Assert.True(jwtTokenService.LastRememberMe);
        Assert.Equal(jwtTokenService.LongExpiration, result.ExpiresAt);
    }

    [Fact]
    public async TaskSystem ForgotPasswordAsync_DoesNotRevealUnknownEmail()
    {
        var repository = new FakeUserRepository();
        var emailPublisher = new FakeEmailMessagePublisher();
        var service = CreateService(repository, emailPublisher: emailPublisher);

        var result = await service.ForgotPasswordAsync(new ForgotPasswordRequestDto
        {
            Email = "missing@example.com"
        });

        Assert.Equal("E-mail verzonden.", result.Message);
        Assert.Equal(0, repository.UpdateCount);
        Assert.Empty(emailPublisher.Messages);
    }

    [Fact]
    public async TaskSystem ForgotPasswordAsync_QueuesResetEmailWithoutExposingToken()
    {
        var user = CreateUser(password: "old-password");
        var repository = new FakeUserRepository(user);
        var emailPublisher = new FakeEmailMessagePublisher();
        var service = CreateService(repository, emailPublisher: emailPublisher);

        var result = await service.ForgotPasswordAsync(new ForgotPasswordRequestDto
        {
            Email = user.Email
        });

        Assert.Equal("E-mail verzonden.", result.Message);
        Assert.Null(result.ResetUrl);
        Assert.Equal(1, repository.UpdateCount);
        Assert.NotNull(user.PasswordResetTokenHash);
        var message = Assert.Single(emailPublisher.Messages);
        Assert.Equal(user.Email, message.ToEmail);
        Assert.Contains("/reset-password?token=", message.TextBody);
    }

    [Fact]
    public async TaskSystem ForgotPasswordAsync_ExposesResetUrlWhenPolicyAllowsIt()
    {
        var user = CreateUser(password: "old-password");
        var emailPublisher = new FakeEmailMessagePublisher();
        var service = CreateService(
            new FakeUserRepository(user),
            emailPublisher: emailPublisher,
            exposeResetUrl: true);

        var result = await service.ForgotPasswordAsync(new ForgotPasswordRequestDto
        {
            Email = user.Email
        });

        Assert.Contains("/reset-password?token=", result.ResetUrl);
        var message = Assert.Single(emailPublisher.Messages);
        Assert.Contains(result.ResetUrl!, message.TextBody);
    }

    [Fact]
    public async TaskSystem ResetPasswordAsync_AcceptsValidTokenAndClearsResetFields()
    {
        var user = CreateUser(password: "old-password");
        var repository = new FakeUserRepository(user);
        var emailPublisher = new FakeEmailMessagePublisher();
        var service = CreateService(repository, emailPublisher: emailPublisher);
        await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = user.Email });
        var resetToken = ExtractResetToken(emailPublisher);

        await service.ResetPasswordAsync(new ResetPasswordRequestDto
        {
            Token = resetToken,
            NewPassword = "new-password",
            ConfirmPassword = "new-password"
        });

        Assert.True(BCrypt.Net.BCrypt.Verify("new-password", user.PasswordHash));
        Assert.Null(user.PasswordResetTokenHash);
        Assert.Null(user.PasswordResetTokenExpiresAt);
    }

    [Fact]
    public async TaskSystem ResetPasswordAsync_RejectsExpiredToken()
    {
        var user = CreateUser(password: "old-password");
        var emailPublisher = new FakeEmailMessagePublisher();
        var service = CreateService(new FakeUserRepository(user), emailPublisher: emailPublisher);
        await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = user.Email });
        var resetToken = ExtractResetToken(emailPublisher);
        user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(-1);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.ResetPasswordAsync(new ResetPasswordRequestDto
        {
            Token = resetToken,
            NewPassword = "new-password",
            ConfirmPassword = "new-password"
        }));
    }

    [Fact]
    public async TaskSystem ChangePasswordAsync_RejectsWrongCurrentPassword()
    {
        var user = CreateUser(password: "old-password");
        var service = CreateService(new FakeUserRepository(user));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.ChangePasswordAsync(user.Id, new ChangePasswordRequestDto
        {
            CurrentPassword = "wrong-password",
            NewPassword = "new-password",
            ConfirmPassword = "new-password"
        }));
    }

    [Fact]
    public async TaskSystem ChangePasswordAsync_AcceptsCurrentPassword()
    {
        var user = CreateUser(password: "old-password");
        var service = CreateService(new FakeUserRepository(user));

        await service.ChangePasswordAsync(user.Id, new ChangePasswordRequestDto
        {
            CurrentPassword = "old-password",
            NewPassword = "new-password",
            ConfirmPassword = "new-password"
        });

        Assert.True(BCrypt.Net.BCrypt.Verify("new-password", user.PasswordHash));
    }

    private static AuthService CreateService(
        FakeUserRepository repository,
        FakeJwtTokenService? jwtTokenService = null,
        FakeEmailMessagePublisher? emailPublisher = null,
        FakeResetPasswordLinkBuilder? resetPasswordLinkBuilder = null,
        bool exposeResetUrl = false)
    {
        return new AuthService(
            repository,
            jwtTokenService ?? new FakeJwtTokenService(),
            emailPublisher ?? new FakeEmailMessagePublisher(),
            resetPasswordLinkBuilder ?? new FakeResetPasswordLinkBuilder(),
            new FakeResetTokenResponsePolicy(exposeResetUrl));
    }

    private static string ExtractResetToken(FakeEmailMessagePublisher emailPublisher)
    {
        var message = Assert.Single(emailPublisher.Messages);
        const string marker = "/reset-password?token=";
        var markerIndex = message.TextBody.IndexOf(marker, StringComparison.Ordinal);
        Assert.True(markerIndex >= 0);

        var tokenStart = markerIndex + marker.Length;
        var tokenEnd = message.TextBody.IndexOfAny(['\r', '\n', ' ', '<'], tokenStart);
        if (tokenEnd < 0)
        {
            tokenEnd = message.TextBody.Length;
        }

        return Uri.UnescapeDataString(message.TextBody[tokenStart..tokenEnd].Trim());
    }

    private static User CreateUser(string password)
    {
        return new User
        {
            Id = 7,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };
    }

    private sealed class FakeJwtTokenService : IJwtTokenService
    {
        public DateTime ShortExpiration { get; } = new(2026, 4, 27, 10, 0, 0, DateTimeKind.Utc);
        public DateTime LongExpiration { get; } = new(2026, 5, 27, 10, 0, 0, DateTimeKind.Utc);
        public bool LastRememberMe { get; private set; }

        public string GenerateToken(User user, bool rememberMe)
        {
            LastRememberMe = rememberMe;
            return rememberMe ? "long-token" : "short-token";
        }

        public DateTime GetExpiration(bool rememberMe)
        {
            return rememberMe ? LongExpiration : ShortExpiration;
        }
    }

    private sealed class FakeEmailMessagePublisher : IEmailMessagePublisher
    {
        public List<EmailMessage> Messages { get; } = new();

        public TaskSystem PublishAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            Messages.Add(message);
            return TaskSystem.CompletedTask;
        }
    }

    private sealed class FakeResetPasswordLinkBuilder : IResetPasswordLinkBuilder
    {
        public string BuildResetPasswordLink(string token)
        {
            return $"https://app.example/reset-password?token={Uri.EscapeDataString(token)}";
        }
    }

    private sealed class FakeResetTokenResponsePolicy : IResetTokenResponsePolicy
    {
        private readonly bool _exposeResetUrl;

        public FakeResetTokenResponsePolicy(bool exposeResetUrl)
        {
            _exposeResetUrl = exposeResetUrl;
        }

        public bool ShouldExposeResetUrl()
        {
            return _exposeResetUrl;
        }
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public FakeUserRepository(params User[] users)
        {
            _users = users.ToList();
        }

        public int UpdateCount { get; private set; }

        public System.Threading.Tasks.Task<User> AddAsync(User user)
        {
            _users.Add(user);
            return System.Threading.Tasks.Task.FromResult(user);
        }

        public System.Threading.Tasks.Task<bool> EmailExistsAsync(string email)
        {
            return System.Threading.Tasks.Task.FromResult(_users.Any(user => string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase)));
        }

        public System.Threading.Tasks.Task<User?> GetByEmailAsync(string email)
        {
            return System.Threading.Tasks.Task.FromResult(_users.FirstOrDefault(user => string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase)));
        }

        public System.Threading.Tasks.Task<User?> GetByIdAsync(int id)
        {
            return System.Threading.Tasks.Task.FromResult(_users.FirstOrDefault(user => user.Id == id));
        }

        public System.Threading.Tasks.Task<User?> GetByResetTokenHashAsync(string resetTokenHash)
        {
            return System.Threading.Tasks.Task.FromResult(_users.FirstOrDefault(user => user.PasswordResetTokenHash == resetTokenHash));
        }

        public TaskSystem UpdateAsync(User user)
        {
            UpdateCount++;
            return TaskSystem.CompletedTask;
        }
    }
}
