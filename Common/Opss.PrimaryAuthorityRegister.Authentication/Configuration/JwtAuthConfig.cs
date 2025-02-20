namespace Opss.PrimaryAuthorityRegister.Authentication.Configuration;

public class JwtAuthConfig
{
    public required Uri IssuerUri { get; set; }
    public required Uri AudienceUri { get; set; }
    public required string SecurityKey { get; set; }
    public short MinutesUntilExpiration { get; set; }
    public short ClockSkewSeconds { get; set; }
}
