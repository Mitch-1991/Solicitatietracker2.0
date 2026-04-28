using SollicitatieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.App.Services.Auth
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, bool rememberMe);
        DateTime GetExpiration(bool rememberMe);
    }
}
