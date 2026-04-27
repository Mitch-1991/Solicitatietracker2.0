using Microsoft.Extensions.Hosting;
using SolicitatieTracker.App.Services.Auth;

namespace Solicitatietracker_API.Services
{
    public class DevelopmentResetTokenResponsePolicy : IResetTokenResponsePolicy
    {
        private readonly IWebHostEnvironment _environment;

        public DevelopmentResetTokenResponsePolicy(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public bool ShouldExposeResetToken()
        {
            return _environment.IsDevelopment();
        }
    }
}
