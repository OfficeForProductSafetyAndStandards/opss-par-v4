using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Client.Extensions;
using Opss.PrimaryAuthorityRegister.Client.Factories;
using Opss.PrimaryAuthorityRegister.Core.Common.Commands.TestCommands;
using Opss.PrimaryAuthorityRegister.Core.Common.Dtos.TestDtos;
using Opss.PrimaryAuthorityRegister.Core.Common.Mediator;
using Opss.PrimaryAuthorityRegister.Core.Common.Queries.TestQueries;

namespace Opss.PrimaryAuthorityRegister.Client.Services;

public class TestClientService : ITestClientService
{
    private readonly HttpClient _httpClient;

    public TestClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpObjectResponse<TestDataDto>> GetTestData(GetTestDataQuery query)
    {
        return await _httpClient.GetAsync<GetTestDataQuery, TestDataDto>(new Uri("api/test/data", UriKind.Relative), query).ConfigureAwait(false);
    }

    public async Task<HttpObjectResponse<CreatedResponse>> CreateTestData(CreateTestDataCommand command)
    {
        return await _httpClient.PostAsync(new Uri("api/test/data", UriKind.Relative), command).ConfigureAwait(false);
    }

    public async Task<HttpObjectResponse<NoContentResult>> PutTestData(UpdateTestDataCommand command)
    {
        return await _httpClient.PutAsync(new Uri("api/test/data", UriKind.Relative), command).ConfigureAwait(false);
    }
}
