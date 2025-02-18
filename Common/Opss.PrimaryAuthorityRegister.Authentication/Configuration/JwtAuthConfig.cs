namespace Opss.PrimaryAuthorityRegister.Authentication.Configuration;

public class JwtAuthConfig
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string SecurityKey { get; set; }
    public short MinutesUntilExpiration { get; set; }
    public short ClockSkewSeconds { get; set; }
}
