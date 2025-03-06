using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Common.Profile.Queries;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Profile.Queries;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Handlers.Common.Profile.Queries
{
    public class GetMyProfileQueryHandlerTests
    {
        private readonly Mock<IGenericRepository<Domain.Entities.Authority>> _repo;
        private ClaimsPrincipal? _claimsPrincipal;

        public GetMyProfileQueryHandlerTests()
        {
            _repo = new Mock<IGenericRepository<Domain.Entities.Authority>>();
        }

        [Fact]
        public async Task WhenNullClaimsPrincipal_ThenUnauthorizedExceptionThrown()
        {
            // Arrange
            _claimsPrincipal = null;
            var handler = new GetMyProfileQueryHandler(_repo.Object, _claimsPrincipal);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                                    handler.Handle(new GetMyProfileQuery(),
                                    CancellationToken.None));

            Assert.Equal("You are not authenticated", exception.Message);
        }
        
        [Fact]
        public async Task WhenNoUserId_ThenUnauthorizedExceptionThrown()
        {
            // Arrange
            _claimsPrincipal = new ClaimsPrincipal();
            var handler = new GetMyProfileQueryHandler(_repo.Object, _claimsPrincipal);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                handler.Handle(new GetMyProfileQuery(),
                    CancellationToken.None));

            Assert.Equal("Your userId was not found", exception.Message);
        }
    }
}
