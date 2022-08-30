<a href="https://www.nuget.org/packages/qckdev.Authentication.JwtBearer"><img src="https://img.shields.io/nuget/v/qckdev.Authentication.JwtBearer.svg" alt="NuGet Version"/></a>
<a href="https://sonarcloud.io/dashboard?id=qckdev.Authentication.JwtBearer"><img src="https://sonarcloud.io/api/project_badges/measure?project=qckdev.Authentication.JwtBearer&metric=alert_status" alt="Quality Gate"/></a>
<a href="https://sonarcloud.io/dashboard?id=qckdev.Authentication.JwtBearer"><img src="https://sonarcloud.io/api/project_badges/measure?project=qckdev.Authentication.JwtBearer&metric=coverage" alt="Code Coverage"/></a>
<a><img src="https://hfrances.visualstudio.com/qckdev/_apis/build/status/qckdev.Authentication.JwtBearer?branchName=master" alt="Azure Pipelines Status"/></a>


# qckdev.Authentication.JwtBearer

Contains classes for working with jwt tokens.

```cs

    using Microsoft.IdentityModel.Tokens;
    using qckdev.Authentication.JwtBearer;
    using System.Security.Claims;
    using System.Text;

    const string TOKEN_SIGNKEY = "<somestring>";
    const double TOKEN_ACCESS_EXPIRE_SECONDS = 86400;

    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TOKEN_SIGNKEY));
    var tokenLifeTimespan = TimeSpan.FromSeconds(TOKEN_ACCESS_EXPIRE_SECONDS);
    var userName = "<username>";
    var roles = new[] { "User", "Admin", "Other" };
    var claims = new Claim[] { new Claim("Key1", "Value1"), new Claim("Key2", "Value") };
    var token = JwtGenerator.CreateToken(key, userName, roles, claims, tokenLifeTimespan);

    return
        new {
            AccessToken = token.AccessToken,
            TokenType = "Bearer",
            ExpiresIn = Math.Ceiling((((DateTime)token.Expired) - DateTime.UtcNow).TotalSeconds),
            RefreshToken = token.RefreshToken,
        };

```