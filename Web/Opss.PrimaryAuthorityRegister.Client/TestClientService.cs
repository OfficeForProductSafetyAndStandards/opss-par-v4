using Opss.PrimaryAuthorityRegister.Core.Common.Dtos;
using Opss.PrimaryAuthorityRegister.Client.Extensions;
using Opss.PrimaryAuthorityRegister.Core.Common.Commands;
using Opss.PrimaryAuthorityRegister.Client.Factories;

namespace Opss.PrimaryAuthorityRegister.Client;

public class TestClientService : ITestClientService
{
    private readonly HttpClient _httpClient;

    public TestClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpObjectResponse<TestDataDto>> GetTestData()
    {
        return await _httpClient.GetAsync<TestDataDto>(new Uri("api/test/data", UriKind.Relative)).ConfigureAwait(false);
    }

    public async Task<HttpObjectResponse> PostTestData(TestDataCommand command)
    {
        return await _httpClient.PostAsync(new Uri("api/test/data", UriKind.Relative), command).ConfigureAwait(false);
    }
}
