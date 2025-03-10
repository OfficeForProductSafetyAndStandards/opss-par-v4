﻿using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Authorisation.ResourceKeyExpanders;

public class MultiplePermissionResourceKeyExpander : IResourceKeyExpander
{
    public IEnumerable<string> GetKeys(string resourceKey)
    {
        ArgumentNullException.ThrowIfNull(resourceKey);

        return resourceKey.Split('|');
    }
}
