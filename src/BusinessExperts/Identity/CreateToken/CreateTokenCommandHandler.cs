using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Orders.Featrures.Create;

public sealed class CreateTokenCommandHandler(IOptions<JwtOptions> options) {
    public async Task<string> Handle(CreateTokenCommand command) {
        var claims = new List<Claim>();
        claims.Add(new(JwtRegisteredClaimNames.Sub, "dev-user"));
        claims.Add(new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));
        claims.Add(new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
        foreach (var scope in options.Value.DevScopes) {
            claims.Add(new Claim("scope", scope));
        }

        var payload = new ClaimsIdentity(claims);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecurityKey!));
        var signature = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtToken = tokenHandler?.CreateJwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(10),
            subject: payload,
            signingCredentials: signature
        );

        return tokenHandler!.WriteToken(jwtToken);
    }
}

public sealed record CreateTokenCommand(
    );

