using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Common.Profile.Queries;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Profile.Queries;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Handlers.Common.Profile.Queries
{
    public class GetMyProfileQueryHandlerTests
    {
        private readonly Mock<IGenericRepository<Domain.Entities.UserProfile>> _mockProfileRepository;
        private ClaimsPrincipal? _claimsPrincipal;

        public GetMyProfileQueryHandlerTests()
        {
            _mockProfileRepository = new Mock<IGenericRepository<Domain.Entities.UserProfile>>();
        }

        [Fact]
        public async Task WhenNullClaimsPrincipal_ThenHttpUnauthorizedExceptionThrown()
        {
            // Arrange
            _claimsPrincipal = null;
            var handler = new GetMyProfileQueryHandler(_mockProfileRepository.Object, _claimsPrincipal);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                                    handler.Handle(new GetMyProfileQuery(),
                                    CancellationToken.None));

            Assert.Equal("You are not authenticated", exception.Message);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, exception.Response.StatusCode);
        }
        
        [Fact]
        public async Task WhenNoUserId_ThenHttpBadRequestExceptionThrown()
        {
            // Arrange
            _claimsPrincipal = new ClaimsPrincipal();
            var handler = new GetMyProfileQueryHandler(_mockProfileRepository.Object, _claimsPrincipal);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                handler.Handle(new GetMyProfileQuery(),
                    CancellationToken.None));

            Assert.Equal("Your userId was not found", exception.Message);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }
        
        [Fact]
        public async Task WhenNoProfile_ThenHttpNotFoundExceptionThrown()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _claimsPrincipal = new ClaimsPrincipal();
            _claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sid, userId.ToString())
            }));
            
            _mockProfileRepository
                .Setup(r => r.GetByIdAsync(It.Is<Guid>(guid => guid == userId)))
                .ReturnsAsync(() => null);
            
            var handler = new GetMyProfileQueryHandler(_mockProfileRepository.Object, _claimsPrincipal);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                handler.Handle(new GetMyProfileQuery(),
                    CancellationToken.None));

            Assert.Equal("Your profile was not found", exception.Message);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.Response.StatusCode);
        }
        
        [Fact]
        public async Task WhenProfileFound_ThenReturnsProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _claimsPrincipal = new ClaimsPrincipal();
            _claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sid, userId.ToString())
            }));

            var profile = new Domain.Entities.UserProfile{HasAcceptedTermsAndConditions = true, Id = userId};

            _mockProfileRepository
                .Setup(r => r.GetByIdAsync(It.Is<Guid>(guid => guid == userId)))
                .ReturnsAsync(() => profile);

            var handler = new GetMyProfileQueryHandler(_mockProfileRepository.Object, _claimsPrincipal);

            // Act
            var result = await handler.Handle(new GetMyProfileQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.HasAcceptedTermsAndConditions);
        }
    }
}
