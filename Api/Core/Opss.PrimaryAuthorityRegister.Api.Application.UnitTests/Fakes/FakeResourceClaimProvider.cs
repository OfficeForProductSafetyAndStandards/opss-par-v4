using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Fakes;

internal class FakeResourceClaimProvider : DemandedClaims, IResourceClaimProvider
{
    public void AddClaimSet(params Claim[] claims){
        AddSatisficingClaimSet(new SatisficingClaimSet("Default", claims.Select(c => new[] { c })));
    }

    public DemandedClaims GetDemandedClaims(object resource)
    {
        return this;
    }
}
