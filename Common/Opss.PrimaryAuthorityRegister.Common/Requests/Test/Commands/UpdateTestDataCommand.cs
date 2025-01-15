using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

public class UpdateTestDataCommand : ICommand
{
    public Guid Id { get; set; }
    public string Data { get; set; }

    public UpdateTestDataCommand(Guid id, string data)
    {
        Id = id;
        Data = data;
    }
}
