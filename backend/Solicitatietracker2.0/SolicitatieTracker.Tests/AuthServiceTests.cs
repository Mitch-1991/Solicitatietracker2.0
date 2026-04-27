using SolicitatieTracker.App.DTOs.Auth;
using SolicitatieTracker.App.Services.Auth;
using SolicitatieTracker.Infrastructure.Data.Repos.Auth;
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
        var service = CreateService(repository);

        var result = await service.ForgotPasswordAsync(new ForgotPasswordRequestDto
        {
            Email = "missing@example.com"
        });

        Assert.Null(result.ResetToken);
        Assert.Null(result.ResetUrl);
        Assert.Equal(0, repository.UpdateCount);
    }

    [Fact]
    public async TaskSystem ForgotPasswordAsync_DoesNotExposeTokenOutsideDevelopment()
    {
        var user = CreateUser(password: "old-password");
        var repository = new FakeUserRepository(user);
        var service = CreateService(repository);

        var result = await service.ForgotPasswordAsync(new ForgotPasswordRequestDto
        {
            Email = user.Email
        });

        Assert.Null(result.ResetToken);
        Assert.Null(result.ResetUrl);
        Assert.Equal(1, repository.UpdateCount);
        Assert.NotNull(user.PasswordResetTokenHash);
    }

    [Fact]
    public async TaskSystem ForgotPasswordAsync_ExposesTokenInDevelopment()
    {
        var user = CreateUser(password: "old-password");
        var service = CreateService(new FakeUserRepository(user), exposeResetToken: true);

        var result = await service.ForgotPasswordAsync(new ForgotPasswordRequestDto
        {
            Email = user.Email
        });

        Assert.NotNull(result.ResetToken);
        Assert.Contains("/reset-password?token=", result.ResetUrl);
    }

    [Fact]
    public async TaskSystem ResetPasswordAsync_AcceptsValidTokenAndClearsResetFields()
    {
        var user = CreateUser(password: "old-password");
        var repository = new FakeUserRepository(user);
        var service = CreateService(repository, exposeResetToken: true);
        var forgotResult = await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = user.Email });

        await service.ResetPasswordAsync(new ResetPasswordRequestDto
        {
            Token = forgotResult.ResetToken!,
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
        var service = CreateService(new FakeUserRepository(user), exposeResetToken: true);
        var forgotResult = await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = user.Email });
        user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(-1);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.ResetPasswordAsync(new ResetPasswordRequestDto
        {
            Token = forgotResult.ResetToken!,
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
        bool exposeResetToken = false)
    {
        return new AuthService(
            repository,
            jwtTokenService ?? new FakeJwtTokenService(),
            new FakeResetTokenResponsePolicy(exposeResetToken));
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

    private sealed class FakeResetTokenResponsePolicy : IResetTokenResponsePolicy
    {
        private readonly bool _exposeResetToken;

        public FakeResetTokenResponsePolicy(bool exposeResetToken)
        {
            _exposeResetToken = exposeResetToken;
        }

        public bool ShouldExposeResetToken()
        {
            return _exposeResetToken;
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
