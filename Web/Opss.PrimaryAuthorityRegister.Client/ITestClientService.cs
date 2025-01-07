using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Client.Factories;
using Opss.PrimaryAuthorityRegister.Core.Common.Commands;
using Opss.PrimaryAuthorityRegister.Core.Common.Dtos;

namespace Opss.PrimaryAuthorityRegister.Client
{
    public interface ITestClientService
    {
        Task<HttpObjectResponse<TestDataDto>> GetTestData();
        Task<HttpObjectResponse> PostTestData(TestDataCommand command);
    }
}