namespace MDR.Data.Model.Jwt;

public class JwtTokenParameterOptions
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}