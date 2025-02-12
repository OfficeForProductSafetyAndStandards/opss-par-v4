using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Common.Authority.Queries;
using Opss.PrimaryAuthorityRegister.Common.Requests.Common.Authority.Queries;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Handlers.Common.Authority.Queries;

public class GetMyOfferedRegulatoryFunctionsQueryHandlerTests
{
    private readonly GetMyOfferedRegulatoryFunctionsQueryHandler _handler;

    public GetMyOfferedRegulatoryFunctionsQueryHandlerTests()
    {
        _handler = new GetMyOfferedRegulatoryFunctionsQueryHandler();
    }

    [Fact]
    public async Task GivenNothing_ThenDataReturned()
    {
        // Arrange
        var request = new GetMyOfferedRegulatoryFunctionsQuery();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Environmental Health", result);
        Assert.Contains("Trading Standards", result);
    }
}
