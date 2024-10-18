using Google.Apis.Auth;
using HotelAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelFull.Server.Service
{
    public class MemberService
    {
        private readonly IConfiguration _configuration;

        public MemberService(IConfiguration configuration)
        {
            _configuration = configuration; // 賦值
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string IdToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _configuration["GoogleKeys:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(IdToken, settings);
                return payload;
            }
            catch
            {
                return null;
            }
        }
        //從過期的token獲取主體信息
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("無效的令牌");

            return principal;
        }

        //產生AccessTokens
        public string GenerateTokens(Member member)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, member.Name),
                new Claim(ClaimTypes.Email , member.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1), // 設置 access token 的有效期
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        //產生RefreshToken
        public string GenerateRefreshToken(Member member)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, member.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var refrashtoken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), // 設置 refresh token 的有效期
                signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(refrashtoken);
        }
    }
}
