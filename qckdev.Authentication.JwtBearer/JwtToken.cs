using System;

namespace qckdev.Authentication.JwtBearer
{
    /// <summary>
    /// Represents a generated JWT token pair.
    /// </summary>
    public sealed class JwtToken
    {
        /// <summary>
        /// Gets or sets the signed JWT access token.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the UTC expiration date of the access token.
        /// </summary>
        public DateTime Expired { get; set; }

        /// <summary>
        /// Gets or sets the generated refresh token.
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
