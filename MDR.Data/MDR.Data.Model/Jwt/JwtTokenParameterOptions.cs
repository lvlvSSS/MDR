using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MDR.Data.Model.Jwt;

public class JwtTokenParameterOptions
{
    public const string Name = "Jwt:Token";

    [Required]
    [StringLength(32, MinimumLength = 16, ErrorMessage = "{2} <= {0} <= {1}")]
    public string? SecretKey { get; set; }

    [Required] public string? Issuer { get; set; }
    [Required] public string? Audience { get; set; }
}