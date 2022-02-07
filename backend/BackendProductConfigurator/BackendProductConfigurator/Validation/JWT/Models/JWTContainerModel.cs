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

        private static JWTContainerModel GetJWTContainerModel(string name, string email)
        {
            return new JWTContainerModel()
            {
                Claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Email, email)
                }
            };
        }
    }
}
