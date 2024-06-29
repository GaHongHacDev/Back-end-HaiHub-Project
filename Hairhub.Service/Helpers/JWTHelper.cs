using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Helpers
{
    public class JWTHelper
    {
        public static string GenerateJwtToken(string apiKeySid, string apiKeySecret)
        {
            var now = DateTime.UtcNow;
            var nowUnixTime = new DateTimeOffset(now).ToUnixTimeSeconds();
            var exp = now.AddHours(1);
            var expUnixTime = new DateTimeOffset(exp).ToUnixTimeSeconds();

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Jti, $"{apiKeySid}-{nowUnixTime}"),
            new Claim(JwtRegisteredClaimNames.Iss, apiKeySid),
            new Claim("rest_api", "true")
        };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiKeySecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(credentials)
        {
            { "cty", "stringee-api;v=1" }
        };

            var payload = new JwtPayload(
                issuer: apiKeySid,
                audience: null,
                claims: claims,
                notBefore: now,
                expires: exp
            );

            var tokenDescriptor = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
