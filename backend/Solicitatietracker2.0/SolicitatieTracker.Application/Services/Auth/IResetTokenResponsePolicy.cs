namespace SolicitatieTracker.App.Services.Auth
{
    public interface IResetTokenResponsePolicy
    {
        bool ShouldExposeResetToken();
    }
}
