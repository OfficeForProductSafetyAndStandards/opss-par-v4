using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Client.Factories;
using Opss.PrimaryAuthorityRegister.Core.Common.Commands.TestCommands;
using Opss.PrimaryAuthorityRegister.Core.Common.Dtos.TestDtos;
using Opss.PrimaryAuthorityRegister.Core.Common.Mediator;
using Opss.PrimaryAuthorityRegister.Core.Common.Queries.TestQueries;

namespace Opss.PrimaryAuthorityRegister.Client.Services
{
    public interface ITestClientService
    {
        Task<HttpObjectResponse<TestDataDto>> GetTestData(GetTestDataQuery query);
        Task<HttpObjectResponse<CreatedResponse>> CreateTestData(CreateTestDataCommand command);
        Task<HttpObjectResponse<NoContentResult>> PutTestData(UpdateTestDataCommand command);
    }
}