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
        private const string SecretKey = "XgXyOPGhX6odhvhk2vOIq4jK2ss9E3LM";
        private const int ExpirationMinutes = 60;
        private HashSet<string> _activeTokens = new HashSet<string>();


        private readonly byte[] _keyBytes;

        public TokenService()
        {
            // Convert the secret key to byte array, because SymmetricSecurityKey needs byte[]
            _keyBytes = Encoding.UTF8.GetBytes(SecretKey);
        }

        public string GenerateToken(int userId, string username)
        {

            // claims are used in authentication, because of their key-value structure
            var claims = new[]
            {
            new Claim("userId", userId.ToString()),
            new Claim("username", username)
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(_keyBytes),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ExpirationMinutes),
                signingCredentials: credentials);


            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _activeTokens.Add(tokenString);
             return tokenString;
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (!_activeTokens.Contains(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_keyBytes),
                ClockSkew = TimeSpan.FromSeconds(30)
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

        public int GetUserIdFromToken(string authHeader)
        {
            if (string.IsNullOrWhiteSpace(authHeader))
                return 0;

            var parts = authHeader.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 || !parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
                return 0;

            string token = parts[1];

            var principal = ValidateToken(token);
            if (principal == null)
                return 0;

            // Find claim "userId"
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return 0;

            if (int.TryParse(userIdClaim.Value, out int userId))
                return userId;

            return 0;
        }

        public bool IsTokenActive(string token)
        {
            return _activeTokens.Contains(token);
        }

        public HashSet<string> getActiveTokens()
        {
            return _activeTokens;
        }

    }
}
