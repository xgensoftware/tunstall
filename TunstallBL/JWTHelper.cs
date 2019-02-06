using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.Security;
using System.IdentityModel.Tokens.Jwt;
namespace TunstallBL
{
    public class JWTHelper
    {
        public static string GetToken(string secret, string username)
        {
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
                             (securityKey, SecurityAlgorithms.HmacSha256Signature);
            
            var header = new JwtHeader(credentials);
            var iat = DateTime.UtcNow;
            var exp = iat.AddMinutes(1440);
            var payload = new JwtPayload
            {
                {"iat", ((DateTimeOffset)iat).ToUnixTimeSeconds()},
                {"exp",  ((DateTimeOffset)exp).ToUnixTimeSeconds()},
                {"username", username }
            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(secToken);
        }
    }
}
