﻿using Opss.PrimaryAuthorityRegister.Api.Domain.Common;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

public class TestData : BaseAuditableEntity
{
    public TestData(Guid ownerId, string data) : base()
    {
        OwnerId = ownerId;
        Data = data;
        DuplicatedData = data;
    }

    public Guid OwnerId { get; set; }
    public string Data { get; set; }
    public string DuplicatedData { get; set; }
}
