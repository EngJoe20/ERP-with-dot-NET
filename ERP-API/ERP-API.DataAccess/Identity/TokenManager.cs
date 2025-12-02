using ERP_API.DataAccess.Entities.User;
using ERP_API.DataAccess.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Identity
{
    internal class TokenManager : ITokenManager
    {
        private readonly JwtOptions _jwtOptions;
        private readonly UserManager<AppUser> _userManager;

        public TokenManager(IOptions<JwtOptions> options, UserManager<AppUser> userManager)
        {
            _jwtOptions = options.Value;
            _userManager = userManager;
        }

        public async Task<TokenResult> GetTokenAsync(AppUser user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new(ClaimTypes.Name, user.UserName));
            claims.Add(new(ClaimTypes.Email, user.Email));
            claims.Add(new(ClaimTypes.NameIdentifier, user.Id));

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new(ClaimTypes.Role, role));
            }

            var encodedKey = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            var key = new SymmetricSecurityKey(encodedKey);

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenExpires = DateTime.Now.AddMinutes(_jwtOptions.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: tokenExpires,
                signingCredentials: credentials
                );

            var tokenHandler = new JwtSecurityTokenHandler();
            return new TokenResult
            {
                Token = tokenHandler.WriteToken(token),
                TokenExpiryTime = tokenExpires,
                Claims = claims.Select(c => new AppClaim { Value = c.Value, Type = c.Type }).ToList()
            };


        }

        public TokenResult GetRefreshToken()
        {
            return new TokenResult
            {
                Token = GenerateRefreshToken(),
                TokenExpiryTime = DateTime.Now.AddHours(_jwtOptions.RefreshTokenExpiryHours)
            };
        }
        public async Task<TokenResult> RefreshToken(string accessToken, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);
            if (user == null)
                throw new Exception("Authentication failed. token is invalid.");
            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new Exception("Authentication failed. refresh token is invalid or expired.");

            return await GetTokenAsync(user);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var encodedKey = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            SecurityKey key = new SymmetricSecurityKey(encodedKey);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = key
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            return principal;
        }

        private string GenerateRefreshToken()
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
