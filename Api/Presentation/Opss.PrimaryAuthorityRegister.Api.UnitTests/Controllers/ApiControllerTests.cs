using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opss.PrimaryAuthorityRegister.Api.Controllers;
using Opss.PrimaryAuthorityRegister.Common;


namespace Opss.PrimaryAuthorityRegister.Api.UnitTests.Controllers;

public class ApiControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ApiController _controller;

    public ApiControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ApiController(_mediatorMock.Object);
    }

    [Fact]
    public async Task ExecutePost_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var requestName = "TestCommand";
        var expectedId = Guid.NewGuid();
        var mockRequest = new TestCommand();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        MockHttpRequestBody(_controller, JsonSerializer.Serialize(mockRequest));

        // Act
        var result = await _controller.ExecutePost(requestName, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<object>>(result);
        var createdResult = Assert.IsType<CreatedResult>(actionResult.Result);
        var actualId = ((CreatedResponse?)createdResult.Value)?.Id;
        Assert.Equal(expectedId, actualId);

        _mediatorMock.Verify(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutePost_ShouldThrowInvalidDataException_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("key", "Invalid data");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.ExecutePost("TestCommand", CancellationToken.None));
    }

    [Fact]
    public async Task ExecutePut_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        var requestName = "TestCommand";
        var mockRequest = new TestCommand();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        MockHttpRequestBody(_controller, JsonSerializer.Serialize(mockRequest));

        // Act
        var result = await _controller.ExecutePut(requestName, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _mediatorMock.Verify(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutePut_ShouldThrowInvalidDataException_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("key", "Invalid data");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.ExecutePut("TestCommand", CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteQuery_ShouldReturnResponse_WhenRequestIsValid()
    {
        // Arrange
        var requestName = "TestQuery";
        var mockRequest = new TestQuery();
        var expectedResponse = new TestQueryResult { Id = Guid.NewGuid(), Data = "Sample Data" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        MockHttpRequestBody(_controller, JsonSerializer.Serialize(mockRequest));

        // Act
        var result = await _controller.ExecuteQuery(requestName, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<object>>(result);
        Assert.Equal(expectedResponse, actionResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteQuery_With_EmptyBody_ShouldReturnResponse_WhenRequestIsValid()
    {
        // Arrange
        var requestName = "TestQuery";
        var expectedResponse = new TestQueryResult { Id = Guid.NewGuid(), Data = "Sample Data" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        MockHttpRequestBody(_controller, "");

        // Act
        var result = await _controller.ExecuteQuery(requestName, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<object>>(result);
        Assert.Equal(expectedResponse, actionResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteQuery_ShouldThrowInvalidDataException_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("key", "Invalid data");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.ExecuteQuery("TestQuery", CancellationToken.None));
    }

    [Fact]
    public async Task PostRequest_ShouldReturnBadRequest_WhenTypeIsNotFound()
    {
        // Act
        var result = await _controller.ExecutePost("NonExistentType", CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<object>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Value);
        Assert.Contains("could not be found", badRequestResult.Value.ToString());
    }

    // Helper to mock HTTP request body
    private void MockHttpRequestBody(ControllerBase controller, string jsonBody)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonBody));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request = { Body = stream }
            }
        };
    }

    // Test classes for mocking purposes
    private class TestCommand { }
    private class TestQuery { }
    private class TestQueryResult
    {
        public Guid Id { get; set; }
        public string Data { get; set; }
    }
}
