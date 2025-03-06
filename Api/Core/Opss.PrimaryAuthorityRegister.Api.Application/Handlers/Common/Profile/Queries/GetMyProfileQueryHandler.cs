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
        private readonly IGenericRepository<Domain.Entities.Authority> _genericRepository;
        private readonly ClaimsPrincipal? _claimsPrincipal;

        public GetMyProfileQueryHandler(IGenericRepository<Domain.Entities.Authority> genericRepository, ClaimsPrincipal? claimsPrincipal)
        {
            _genericRepository = genericRepository;
            _claimsPrincipal = claimsPrincipal;
        }

        public Task<MyProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            if (_claimsPrincipal == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized, "You are not authenticated");

            var userId = _claimsPrincipal.GetUserId();
            if (userId == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized, "Your userId was not found");

            return new Task<MyProfileDto>(null);
        }
    }
}
