namespace SolicitatieTracker.Infrastructure.Messaging;

public interface IResetPasswordLinkBuilder
{
    string BuildResetPasswordLink(string token);
}
