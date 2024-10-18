using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelFull.Server.Models;

namespace HotelFull.Server.Filters
{
    public class EmployeeJwtAuthFilter
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ILogger<EmployeeJwtAuthFilter> _logger;

        public EmployeeJwtAuthFilter(IOptions<JwtSettings> jwtSettings, ILogger<EmployeeJwtAuthFilter> logger)
        {
            _logger = logger;

            // JWT 設定驗證
            if (jwtSettings == null || jwtSettings.Value == null)
            {
                _logger.LogError("JWT settings are null");
                throw new ArgumentNullException(nameof(jwtSettings));
            }

            if (string.IsNullOrEmpty(jwtSettings.Value.Key))
            {
                _logger.LogError("JWT key is null or empty");
                throw new ArgumentException("JWT key cannot be null or empty", nameof(jwtSettings));
            }

            //Token 驗證參數配置
            var key = Encoding.ASCII.GetBytes(jwtSettings.Value.Key);
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero // Optional: Reduce clock skew for token expiration
            };
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context) //實現了授權邏輯，處理傳入請求的身份驗證
        {
            //獲取 Token
            var token = GetTokenFromHeader(context);
            if (string.IsNullOrWhiteSpace(token))
            {
                SetUnauthorizedResult(context, "Token is missing.");
                return;
            }

            //驗證 Token
            try
            {
                var principal = ValidateToken(token);
                if (principal == null)
                {
                    SetUnauthorizedResult(context, "Invalid token.");
                    return;
                }

                var nameClaim = principal.FindFirst(ClaimTypes.Name);
                if (nameClaim == null || string.IsNullOrWhiteSpace(nameClaim.Value))
                {
                    SetUnauthorizedResult(context, "Token validation failed.");
                    return;
                }

                var roleClaim = principal.FindFirst(ClaimTypes.Role);
                if (string.IsNullOrWhiteSpace(roleClaim?.Value) ||
                    !new[] { "Admin", "Employee", "Manager" }.Contains(roleClaim.Value))
                {
                    SetUnauthorizedResult(context, "Only Admin can perform this action.");
                    return;
                }

                context.HttpContext.User = principal; // Set the authenticated user
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token validation failed: {ex.Message}");
                SetUnauthorizedResult(context, "Token validation failed.");
            }

            await Task.CompletedTask; // Ensure the method is asynchronous
        }

        private string GetTokenFromHeader(AuthorizationFilterContext context)
        {
            return context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);

            // Additional checks can be done here, e.g., ensuring validatedToken is a JwtSecurityToken
            return validatedToken is JwtSecurityToken ? principal : null;
        }

        private void SetUnauthorizedResult(AuthorizationFilterContext context, string reason)
        {
            context.Result = new UnauthorizedObjectResult(new { message = reason });
        }
    }
}
