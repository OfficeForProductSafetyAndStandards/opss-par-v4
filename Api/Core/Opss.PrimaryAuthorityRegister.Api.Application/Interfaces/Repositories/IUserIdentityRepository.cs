using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;

public interface IUserIdentityRepository
{
    UserIdentity? GetUserIdentiyByEmail(string email);
}
