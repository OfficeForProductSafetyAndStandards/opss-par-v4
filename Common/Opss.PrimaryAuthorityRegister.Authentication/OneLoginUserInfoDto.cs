using Newtonsoft.Json;

namespace Opss.PrimaryAuthorityRegister.Authentication;

public class OneLoginUserInfoDto
{
    [JsonProperty("sub")]
    public string Sub { get; set; }
    [JsonProperty("email")]
    public string? Email { get; set; }
    [JsonProperty("email_verified")]
    public string? EmailVerified { get; set; }
    [JsonProperty("phone_number")]
    public string? PhoneNumber { get; set; }
    [JsonProperty("phone_number_verified")]
    public string? PhoneNumberVerified { get; set; }
    [JsonProperty("updated_at")]
    public string UpdatedAt { get; set; }

    public OneLoginUserInfoDto(string sub, string updatedAt)
    {
        Sub = sub;
        UpdatedAt = updatedAt;
    }
}
