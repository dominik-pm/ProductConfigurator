using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BackendProductConfigurator.Validation.JWT.Models
{
    public class JWTContainerModel : IAuthContainerModel
    {
        public string SecretKey { get; set; } = "c2plaDkzdWhBVWhpdW9zZGg5ODhob2lBdWgz";
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public int ExpireMinutes { get; set; } = 10080; // 7 days
        public Claim[] Claims { get; set; }

        public static JWTContainerModel GetJWTContainerModel(string name, string email, bool admin)
        {
            return new JWTContainerModel()
            {
                Claims = new Claim[]
                {
                    new Claim("userName", name),
                    new Claim("userEmail", email),
                    new Claim("admin", admin.ToString(), ClaimValueTypes.Boolean)
                }
            };
        }
    }
}
