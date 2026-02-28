using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace qckdev.Authentication.JwtBearer
{
    /// <summary>
    /// Provides helper methods to create and validate JWT tokens.
    /// </summary>
    public static class JwtGenerator
    {

        /// <summary>
        /// Creates a signed JWT access token and an associated refresh token.
        /// </summary>
        /// <param name="key">Security key used to sign the token.</param>
        /// <param name="userName">User name to include as token identifier.</param>
        /// <param name="roles">User roles to include as role claims.</param>
        /// <param name="claims">Additional claims to include in the token.</param>
        /// <param name="lifespan">Token lifetime. When null, one day is used.</param>
        /// <returns>A <see cref="JwtToken"/> containing the generated tokens and expiration.</returns>
        public static JwtToken CreateToken(SecurityKey key, string userName, IEnumerable<string> roles = null, IEnumerable<Claim> claims = null, TimeSpan? lifespan = null)
        {
            var tokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, userName)
            };
            roles?.ToList().ForEach(rol => tokenClaims.Add(new Claim(ClaimTypes.Role, rol)));
            claims?.ToList().ForEach(val => tokenClaims.Add(val));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(tokenClaims),
                Expires = DateTime.UtcNow.Add(lifespan ?? TimeSpan.FromDays(1)),
                SigningCredentials = credentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);
            return new JwtToken
            {
                AccessToken = tokenHandler.WriteToken(token),
                Expired = token.ValidTo,
                RefreshToken = $"1/{CreateGenericToken()}"
            };
        }

        /// <summary>
        /// Validates a JWT token using the provided validation parameters.
        /// </summary>
        /// <param name="validationParameters">Parameters used during token validation.</param>
        /// <param name="token">JWT token to validate.</param>
        /// <returns>The validated token as <see cref="JwtSecurityToken"/>.</returns>
        public static JwtSecurityToken ValidateToken(TokenValidationParameters validationParameters, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return (JwtSecurityToken)validatedToken;
        }

        /// <summary>
        /// Creates a random token string that is safe to use in route segments.
        /// </summary>
        /// <returns>A base64 token string with '/' replaced by '!'.</returns>
        public static string CreateGenericToken()
        {
            var randomNumber = GetRandomSerie();

            return Convert.ToBase64String(randomNumber)
                //Avoid trouble with routes
                .Replace("/", "!");
        }

        private static byte[] GetRandomSerie()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return randomNumber;
        }

    }
}
