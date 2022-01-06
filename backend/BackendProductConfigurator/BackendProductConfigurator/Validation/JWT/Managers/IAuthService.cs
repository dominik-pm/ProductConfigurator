using BackendProductConfigurator.Validation.JWT.Models;
using System.Security.Claims;

namespace BackendProductConfigurator.Validation.JWT.Managers
{
    public interface IAuthService
    {
        string SecretKey { get; set; }

        bool IsTokenValid(string token);
        string GenerateToken(IAuthContainerModel model);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
