using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace MediaRatingPlatform_BusinessLogicLayer
{
    public class TokenService
    {
        private const string SecretKey = "xuXBuxDmUc8Dc0dhVinCzSwZl2OXQIyb";
        private const int ExpirationMinutes = 60;

        private readonly byte[] _keyBytes;

        public TokenService()
        {
            _keyBytes = Encoding.UTF8.GetBytes(SecretKey);
        }

        // Generate a JWT
        public string GenerateToken(int userId, string username)
        {
            var claims = new[]
            {
            new Claim("userId", userId.ToString()),
            new Claim("username", username)
            // add roles or other claims if needed
        };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(_keyBytes),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ExpirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Validate token and get ClaimsPrincipal (null if invalid)
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_keyBytes),
                ClockSkew = TimeSpan.FromSeconds(30) // small grace
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParams, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
