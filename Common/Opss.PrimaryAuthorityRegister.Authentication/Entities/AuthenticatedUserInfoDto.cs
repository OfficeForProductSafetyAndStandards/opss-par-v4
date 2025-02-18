using System.Text.Json.Serialization;

namespace Opss.PrimaryAuthorityRegister.Authentication.Entities;

public class AuthenticatedUserInfoDto
{
    [JsonPropertyName("sub")]
    public string Sub { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("email_verified")]
    public bool? EmailVerified { get; set; }
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
    [JsonPropertyName("phone_number_verified")]
    public bool? PhoneNumberVerified { get; set; }
    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; }

    public AuthenticatedUserInfoDto(string sub, string updatedAt)
    {
        Sub = sub;
        UpdatedAt = updatedAt;
    }
}
