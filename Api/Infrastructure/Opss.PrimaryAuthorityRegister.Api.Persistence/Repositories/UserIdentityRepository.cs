using Microsoft.EntityFrameworkCore;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Repositories;

public class UserIdentityRepository : IUserIdentityRepository
{
    private readonly IGenericRepository<UserIdentity> _repository;

    public UserIdentityRepository(IGenericRepository<UserIdentity> repository)
    {
        _repository = repository;
    }
    public UserIdentity? GetUserIdentiyByEmail(string email)
    {
        return _repository.Entities
                          .Include(u => u.Roles)
                          .SingleOrDefault(u => u.EmailAddress == email);
    }
}