using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Http;

namespace Opss.PrimaryAuthorityRegister.Http.UnitTests;

public class ResponsesTests
{
    [Fact]
    public void NotFound_ShouldReturnNotFoundObjectResult_WithNullError()
    {
        // Act
        var result = Responses.NotFound();

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Null(result.Value);
    }

    [Fact]
    public void NotFound_ShouldReturnNotFoundObjectResult_WithError()
    {
        // Arrange
        var error = "Error message";

        // Act
        var result = Responses.NotFound(error);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(error, result.Value);
    }

    [Fact]
    public void BadRequest_ShouldReturnBadRequestObjectResult_WithNullError()
    {
        // Act
        var result = Responses.BadRequest();

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Null(result.Value);
    }

    [Fact]
    public void BadRequest_ShouldReturnBadRequestObjectResult_WithError()
    {
        // Arrange
        var error = "Error message";

        // Act
        var result = Responses.BadRequest(error);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(error, result.Value);
    }

    [Fact]
    public void Created_ShouldReturnCreatedResult_WithGuid()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = Responses.Created(id);

        // Assert
        Assert.IsType<CreatedResult>(result);
        Assert.Equal(string.Empty, result.Location);
        Assert.IsType<CreatedResponse>(result.Value);
        Assert.Equal(id, ((CreatedResponse)result.Value).Id);
    }

    [Fact]
    public void Created_ShouldReturnCreatedResult_WithMultipleGuids()
    {
        // Arrange
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        var result = Responses.Created(ids);

        // Assert
        Assert.IsType<CreatedResult>(result);
        Assert.Equal(string.Empty, result.Location);
        var responseValues = Assert.IsAssignableFrom<IEnumerable<CreatedResponse>>(result.Value);
        Assert.Equal(ids.Count, responseValues.Count());
        Assert.All(responseValues, r => Assert.Contains(r.Id, ids));
    }

    [Fact]
    public void Created_WithLocationAndGuid_ShouldReturnCreatedResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var location = "/resource/123";

        // Act
        var result = Responses.Created(location, id);

        // Assert
        Assert.IsType<CreatedResult>(result);
        Assert.Equal(location, result.Location);
        Assert.IsType<CreatedResponse>(result.Value);
        Assert.Equal(id, ((CreatedResponse)result.Value).Id);
    }

    [Fact]
    public void Created_WithLocationAndMultipleGuids_ShouldReturnCreatedResult()
    {
        // Arrange
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var location = "/resource/123";

        // Act
        var result = Responses.Created(location, ids);

        // Assert
        Assert.IsType<CreatedResult>(result);
        Assert.Equal(location, result.Location);
        var responseValues = Assert.IsAssignableFrom<IEnumerable<CreatedResponse>>(result.Value);
        Assert.Equal(ids.Count, responseValues.Count());
        Assert.All(responseValues, r => Assert.Contains(r.Id, ids));
    }

    [Fact]
    public void NoContent_ShouldReturnNoContentResult()
    {
        // Act
        var result = Responses.NoContent();

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void Ok_ShouldReturnOkResult()
    {
        // Act
        var result = Responses.Ok();

        // Assert
        Assert.IsType<OkResult>(result);
    }
}
