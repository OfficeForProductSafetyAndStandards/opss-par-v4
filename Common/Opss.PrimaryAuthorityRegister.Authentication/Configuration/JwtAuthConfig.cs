namespace Opss.PrimaryAuthorityRegister.Authentication.Configuration;

public class JwtAuthConfig
{
    /// <summary>
    /// The URI of the token issuer (e.g., the identity provider).
    /// This value is used to validate the `iss` claim in incoming tokens.
    /// </summary>
    public required Uri IssuerUri { get; set; }

    /// <summary>
    /// The expected audience for the token, typically the API that will consume it.
    /// This value is used to validate the `aud` claim in incoming tokens.
    /// </summary>
    public required Uri AudienceUri { get; set; }

    /// <summary>
    /// The secret key or signing key used to validate the token signature.
    /// This should be kept secure and should be long enough for strong security.
    /// </summary>
    public required string SecurityKey { get; set; }

    /// <summary>
    /// The duration (in minutes) for which a generated token remains valid before expiration.
    /// </summary>
    public short MinutesUntilExpiration { get; set; }

    /// <summary>
    /// The allowed clock skew (in seconds) to account for time differences between servers.
    /// This helps prevent token rejection due to minor clock mismatches.
    /// </summary>
    public short ClockSkewSeconds { get; set; }
}
