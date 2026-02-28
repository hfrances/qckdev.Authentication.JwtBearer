using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace qckdev.Authentication.JwtBearer.Test
{
    [TestClass]
    public class JwtGeneratorTest
    {
        private static SymmetricSecurityKey CreateKey(string seed)
        {
            var keyBytes = Encoding.UTF8.GetBytes(seed.PadRight(64, 'x'));
            return new SymmetricSecurityKey(keyBytes);
        }

        [TestMethod]
        public void CreateToken_BasicValues_AreGenerated()
        {
            var key = CreateKey("test-signing-key");

            var token = JwtGenerator.CreateToken(key, "user1");

            Assert.IsFalse(string.IsNullOrWhiteSpace(token.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(token.RefreshToken));
            Assert.IsTrue(token.RefreshToken.StartsWith("1/", StringComparison.Ordinal));
        }

        [TestMethod]
        public void CreateToken_DefaultLifetime_IsAroundOneDay()
        {
            var key = CreateKey("default-lifetime-key");
            var startUtc = DateTime.UtcNow;

            var token = JwtGenerator.CreateToken(key, "user1");
            var delta = token.Expired - startUtc;

            Assert.IsTrue(delta > TimeSpan.FromHours(23), $"Expected lifetime > 23h, actual: {delta}.");
            Assert.IsTrue(delta < TimeSpan.FromHours(25), $"Expected lifetime < 25h, actual: {delta}.");
        }

        [TestMethod]
        public void CreateToken_ContainsExpectedClaims()
        {
            var key = CreateKey("claims-key");
            var roles = new[] { "Admin", "User" };
            var extraClaims = new[]
            {
                new Claim("tenant", "qckdev"),
                new Claim("region", "eu")
            };

            var token = JwtGenerator.CreateToken(key, "user1", roles, extraClaims, TimeSpan.FromMinutes(30));
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.AccessToken);

            Assert.AreEqual("user1", jwt.Claims.Single(x => x.Type == JwtRegisteredClaimNames.NameId).Value);
            CollectionAssert.AreEquivalent(
                roles,
                jwt.Claims
                    .Where(x => x.Type == ClaimTypes.Role || x.Type == "role")
                    .Select(x => x.Value)
                    .ToArray()
            );
            Assert.AreEqual("qckdev", jwt.Claims.Single(x => x.Type == "tenant").Value);
            Assert.AreEqual("eu", jwt.Claims.Single(x => x.Type == "region").Value);
        }

        [TestMethod]
        public void ValidateToken_WithCorrectKey_ReturnsValidatedToken()
        {
            var key = CreateKey("validation-key");
            var token = JwtGenerator.CreateToken(key, "user1", lifespan: TimeSpan.FromMinutes(10));
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key
            };

            var validatedToken = JwtGenerator.ValidateToken(parameters, token.AccessToken);

            Assert.IsNotNull(validatedToken);
            Assert.AreEqual("user1", validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.NameId).Value);
        }

        [TestMethod]
        public void ValidateToken_WithWrongKey_ThrowsInvalidSignature()
        {
            var key = CreateKey("right-key");
            var wrongKey = CreateKey("wrong-key");
            var token = JwtGenerator.CreateToken(key, "user1", lifespan: TimeSpan.FromMinutes(10));
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = wrongKey
            };

            try
            {
                JwtGenerator.ValidateToken(parameters, token.AccessToken);
                Assert.Fail("Expected token validation to fail with an invalid signature.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(
                    ex is SecurityTokenInvalidSignatureException || ex is SecurityTokenSignatureKeyNotFoundException,
                    $"Unexpected exception type: {ex.GetType().FullName}"
                );
            }
        }

        [TestMethod]
        public void CreateGenericToken_DoesNotContainSlash_AndRepresents32Bytes()
        {
            var token = JwtGenerator.CreateGenericToken();

            Assert.IsFalse(token.Contains("/"), "Token should not contain '/' to avoid route issues.");

            var raw = Convert.FromBase64String(token.Replace("!", "/"));
            Assert.AreEqual(32, raw.Length);
        }
    }
}
