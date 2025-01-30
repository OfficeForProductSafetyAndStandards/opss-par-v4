using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;

public interface ITestDataRepository
{
    public Task<List<TestData>> GetByOwnerId(Guid ownerId);
}