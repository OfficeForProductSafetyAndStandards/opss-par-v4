using Microsoft.EntityFrameworkCore;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Authorisation.ResourceKeyExpanders;

public class TestDataResourceKeyExpander : IResourceKeyExpander
{
    private readonly IUnitOfWork _testDataRepository;

    public TestDataResourceKeyExpander(IUnitOfWork testDataRepository)
    {
        _testDataRepository = testDataRepository;
    }

    /// <summary>
    /// Expand the resource key from the Command / Query and return claims that can satisfy it.
    /// e.g. You're requesting permissions for a given bit of test data, this can be expanded to
    /// include the owner of the test data as well, so return those alternative required claims as well.
    /// </summary>
    /// <param name="resourceKey"></param>
    /// <returns></returns>
    public IEnumerable<string> GetKeys(string resourceKey)
    {
        ArgumentNullException.ThrowIfNull(resourceKey);

        var keyDetails = resourceKey.Split('/');

        if (keyDetails[0] != "TestData") return [];

        // If * then we can look at ALL 
        if (keyDetails[1] == "*")
        {
            var allItems = _testDataRepository.Repository<TestData>().Entities.ToListAsync().Result;

            var allTestData = allItems
                 .Select(i => $"TestData/{i.Id}")
                 .Concat(allItems.Select(i => $"TestData/Owner/{i.OwnerId}"))
                 .Distinct()
                 .ToList();

            return allTestData;
        }

        Guid TestDataId;
        if (!Guid.TryParse(keyDetails[1], out TestDataId)) return [];

        var ownedItems = _testDataRepository.Repository<TestData>().GetByIdAsync(Guid.Parse(keyDetails[1])).Result;

        if (ownedItems == null) return [];

        return new List<string> { $"TestData/Owner/{ownedItems.OwnerId.ToString()}" };
    }
}
