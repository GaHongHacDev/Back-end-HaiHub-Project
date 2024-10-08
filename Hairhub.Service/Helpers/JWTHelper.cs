using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Helpers
{
    public class JWTHelper
    {
        private readonly IConfiguration _configuaration;

        public JWTHelper(IConfiguration configuaration)
        {
            _configuaration = configuaration;
        }

        /*public static string GenerateJwtToken(string apiKeySid, string apiKeySecret)
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
*/

        public static string GenerateToken(string username, string roleName, string JWTkey, string JWTIssuer, string JWTAudience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(JWTkey); //_configuaration["JWTSettings:Key"]

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, roleName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                Issuer = JWTIssuer, //_configuaration["JWTSettings:Issuer"],
                Audience = JWTAudience, //_configuaration["JWTSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
