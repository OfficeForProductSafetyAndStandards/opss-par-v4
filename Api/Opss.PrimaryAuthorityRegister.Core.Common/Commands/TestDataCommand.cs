using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Core.Common.Dtos;

namespace Opss.PrimaryAuthorityRegister.Core.Common.Commands;

public class TestDataCommand : IRequest<ActionResult>
{
    public TestDataCommand(TestDataDto data)
    {
        Data = data;
    }

    public TestDataDto Data { get; private set; }
}
