namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Queries.Dtos;

public class TestDataDto
{
    public Guid Id { get; set; }
    public string Data { get; set; }

    public TestDataDto(Guid id, string data)
    {
        Id = id;
        Data = data;
    }
}
