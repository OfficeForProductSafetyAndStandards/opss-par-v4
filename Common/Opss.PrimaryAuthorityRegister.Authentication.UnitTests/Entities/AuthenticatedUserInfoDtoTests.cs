using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using System.Text.Json;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.Entities;

public class AuthenticatedUserInfoDtoTests
{
    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    [Fact]
    public void Constructor_ShouldInitializeRequiredFields()
    {
        // Arrange & Act
        var dto = new AuthenticatedUserInfoDto("subject-123", "2024-01-01T12:00:00Z");

        // Assert
        Assert.Equal("subject-123", dto.Sub);
        Assert.Equal("2024-01-01T12:00:00Z", dto.UpdatedAt);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var dto = new AuthenticatedUserInfoDto("subject-123", "2024-01-01T12:00:00Z")
        {
            Email = "test@example.com",
            EmailVerified = true,
            PhoneNumber = "+1234567890",
            PhoneNumberVerified = "false"
        };

        // Assert
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal(true, dto.EmailVerified);
        Assert.Equal("+1234567890", dto.PhoneNumber);
        Assert.Equal("false", dto.PhoneNumberVerified);
    }

    [Fact]
    public void ShouldSerializeToJsonWithCorrectPropertyNames()
    {
        // Arrange
        var dto = new AuthenticatedUserInfoDto("subject-123", "2024-01-01T12:00:00Z")
        {
            Email = "test@example.com",
            EmailVerified = true,
            PhoneNumber = "+1234567890",
            PhoneNumberVerified = "false"
        };

        // Act
        var json = JsonSerializer.Serialize(dto, options);

        // Assert
        Assert.Contains("\"sub\":\"subject-123\"", json);
        Assert.Contains("\"email\":\"test@example.com\"", json);
        Assert.Contains("\"email_verified\":\"true\"", json);
        Assert.Contains("\"phone_number\":\"+1234567890\"", json);
        Assert.Contains("\"phone_number_verified\":\"false\"", json);
        Assert.Contains("\"updated_at\":\"2024-01-01T12:00:00Z\"", json);
    }

    [Fact]
    public void ShouldDeserializeFromJsonWithCorrectPropertyNames()
    {
        // Arrange
        var json = "{\"sub\":\"subject-123\",\"email\":\"test@example.com\",\"email_verified\":\"true\",\"phone_number\":\"+1234567890\",\"phone_number_verified\":\"false\",\"updated_at\":\"2024-01-01T12:00:00Z\"}";

        // Act
        var dto = JsonSerializer.Deserialize<AuthenticatedUserInfoDto>(json, options);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("subject-123", dto.Sub);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal(true, dto.EmailVerified);
        Assert.Equal("+1234567890", dto.PhoneNumber);
        Assert.Equal("false", dto.PhoneNumberVerified);
        Assert.Equal("2024-01-01T12:00:00Z", dto.UpdatedAt);
    }
}
