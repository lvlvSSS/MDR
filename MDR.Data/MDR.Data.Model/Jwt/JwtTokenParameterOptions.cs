using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;

namespace MDR.Data.Model.Jwt;

public class JwtTokenParameterOptions
{
    public const string Name = "Jwt:Token";
    public static readonly Encoding DefaultEncoding = Encoding.UTF8;
    public static readonly double DefaultExpiresMinutes = 30d;

    [Required]
    [StringLength(32, MinimumLength = 16, ErrorMessage = "{2} <= {0} <= {1}")]
    public string? SecretKey { get; set; }

    [Required] public string? Issuer { get; set; }
    public string? Audience { get; set; }

    public double ExpiresMinutes { get; set; } = DefaultExpiresMinutes;

    public Encoding Encoding { get; set; } = DefaultEncoding;
    public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.GetBytes(SecretKey ?? "1234567890abcdef"));

    public TokenValidationParameters DefaultTokenValidationParameters
    {
        get
        {
            return new TokenValidationParameters
            {
                //ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256, SecurityAlgorithms.RsaSha256 },
                ValidTypes = new[] { JwtConstants.HeaderType },


                ValidIssuer = this.Issuer,
                ValidateIssuer = true,

                ValidAudience = this.Audience,
                ValidateAudience = false,

                IssuerSigningKey = this.SymmetricSecurityKey,
                ValidateIssuerSigningKey = true,

                ValidateLifetime = true,

                RequireSignedTokens = true,
                RequireExpirationTime = true,

                NameClaimType = JwtClaimTypes.Name,
                RoleClaimType = JwtClaimTypes.Role,

                ClockSkew = TimeSpan.Zero,
            };
        }
    }

    public SigningCredentials DefaultSigningCredentials =>
        new(this.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
}