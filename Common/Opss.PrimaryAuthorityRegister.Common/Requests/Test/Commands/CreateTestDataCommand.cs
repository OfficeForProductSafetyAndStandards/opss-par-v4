using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

public class CreateTestDataCommand : ICommand<Guid>
{
    public CreateTestDataCommand(string data)
    {
        Data = data;
    }
    public string Data { get; }
}