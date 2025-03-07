using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Profile.Queries;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Common.Profile.Queries
{
    public class GetMyProfileQueryHandler
        : IRequestHandler<GetMyProfileQuery, MyProfileDto>
    {
        private readonly IGenericRepository<Domain.Entities.UserProfile> _profileRepository;
        private readonly ClaimsPrincipal? _claimsPrincipal;

        public GetMyProfileQueryHandler(IGenericRepository<Domain.Entities.UserProfile> profileRepository, ClaimsPrincipal? claimsPrincipal)
        {
            _profileRepository = profileRepository;
            _claimsPrincipal = claimsPrincipal;
        }

        public async Task<MyProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            if (_claimsPrincipal == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized, "You are not authenticated");

            var userId = _claimsPrincipal.GetUserId();
            if (userId == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest, "Your userId was not found");

            var profile = await _profileRepository.GetByIdAsync(userId.Value).ConfigureAwait(false);
            if (profile == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound, "Your profile was not found");

            var profileDto = new MyProfileDto(profile.HasAcceptedTermsAndConditions);

            return profileDto;
        }
    }
}
