﻿using Opss.PrimaryAuthorityRegister.Authentication.Configuration;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests;

internal static class AuthenticationTestHelpers
{
    internal static string StaticTestRsaKey = "-----BEGIN PRIVATE KEY-----MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDCjsiQBY+xZVpihpeJECwQEfar9bpmX5jg7t3EOJZ6y8b4zUsmMbPiJulDtoDuNjoP49j21eEFPeJJSIckCql1Wt1eQWc56vx9iHnZS+f++XxQkOxMyWz+o5bz+P8Rp/KZ90JuYS0VSgs5YBYYBBPvqRUkp8sdfiMlMwrhrpNZdNiJw/inclRwbvZ4A8HF4eAFecY3Xz1X/9SmhMbfqqG2W0keO3/TbUr/jTaFA3rM+no3S7q6xJmrxGYFfsvalgDRxudKMMzSw1RVo2Uli//mQVf0pPZOLAF+7paq/vza+PCl4dGp0MhKz0WT7zfkKUswgDTohHC+VTMcAZnHFo2pAgMBAAECggEAH7sKwdZuW4fEqHn7/+onzY0odl4ywtyHgfPjLkvuWuOeNVjCD14e0Nz4wUbkGzWz3YCTE5rJQsIXhE00YS/t+EpT/TncfIkzGcQm30YudZq56CfiqhlV0efbSDoNW5NREUROzNLDeAvl7bsaj1sm5zjjzmEhUtOOJtR+y1TeCkInrT4enr3OZegF714EcywEDhzH+5BnULGggP97c1v/CujqZ865AN2RN9EkNWOEHMbywetP8V8QyzOPk5dBuB4fkp8B0uUQwTaQCr/OAtRBX+7E05kLfp5jA5jCBoOlw49phY3L0azdXETQJnMmACJ86NhYrjeMpPC/iImjHl9CawKBgQD0xY8/tRr57YNIrMpZ8T86RRRFQGWXcGZMOuvaVq4QM637EId3kUeMyTaJi/+/yEjIFbjJNPhCYtltxTU4bgPNH0ORZsDCRyl++0sQpj2xggVFBPPQe36kfQ6yTLktwS33jDEpg3eWHoRePzKDOZ7ThlU7jON26lJcLspI9qeWDwKBgQDLe4snH9ygG5wnSayvnn63L04valZ5E/6Hh/e2D/m+mbTzHklGexz2hhYy9Ci8GeCXTBjnkQ4SCUJ3zGSDZ2bGNihb6hlDYuN9D+i5aVky84GGjQRc+mcZlfHpiP9BNc+wfmiFAJ8JQvVBw3qysgadw0Mp9wRpZF2/ATYH0siYxwKBgDxmc87YtetufLL2UIiZS2zplvLvzSHtjpDJCWI7eYBuAESv833Bz9Ih5N9UOKvulGcrVQnxlEFtexHnVBa0ryNyz42VuYM3ZDn9cKyPGTIwT3SUoEWV885LPdEptZhgzyMC6S7BTkUxCqDnH2PaWCMCRw4G2iqB8AjnUutmUjxpAoGAZ+9T1YklxTY1HbA5H38ilGj6U6fKQZAf65Rcx8cDNwMF9UScAv8xfQ5iWmZyRBonqMA63FUwTbAjHlPtZ10ylr3lAYXin5PsRN4SblpMMIVGvLZc6y0P6na3mSTb1LAqxKjctErr8OwdoBi8HHhofr7VGKNOwpJ8e+qfcYGN6tMCgYAXVRBNUKP9TVvSn79cb4lmeEBwrsHTN/F+3IPHOxevw62/1EaagvoCHlURo9TaHy4+i3v9kIWakbf8NrQG7Z5kTZi7GOiEwvhWHOqld8YPI8TU2I6jb+y+yJyDlAOuJJH03F+TRLNG0P7VBA+1UUkwaYkjlFZ8+MH/ZizvaOS23w==-----END PRIVATE KEY-----";

    internal static string AuthProviderKey = "Provider";

    internal static JwtAuthConfig JwtConfig = new JwtAuthConfig
    {
        AudienceUri = new Uri("https://audience.example.com"),
        IssuerUri = new Uri("https://issuer.example.com"),
        SecurityKey = "MDEyMzQ1Njc4OWFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6MTIzNDU2Nzg5",
        ClockSkewSeconds = 340,
        MinutesUntilExpiration = 30
    };

    internal static OpenIdConnectAuthConfiguration ProviderAuthConfig = new OpenIdConnectAuthConfiguration
    {
        ProviderKey = AuthProviderKey,
        AuthorityUri = new Uri("https://example.com"),
        IssuerUri = new Uri("https://issuer.example.com"),
        ClientId = "client-id",
        CookieMaxAge = 60,
        ClockSkewSeconds = 300,
        PostLogoutRedirectUri = new Uri("https://localhost/"),
        RsaPrivateKey = StaticTestRsaKey,
        WellKnownPath = "/.well-known/openid-configuration",
        UserInfoPath = "/userinfo",
        AccessTokenPath = "/accesstoken",
        CallbackPath = $"/{AuthProviderKey.ToLower()}-signin-oidc",
        ClientSecret = "MDEyMzQ1Njc4OWFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6MTIzNDU2Nzg5"
    };

    internal static OpenIdConnectAuthConfigurations ProviderAuthConfigurations = new OpenIdConnectAuthConfigurations(
        new Dictionary<string, OpenIdConnectAuthConfiguration> {
        {
            AuthProviderKey, ProviderAuthConfig
        }
    });
}
