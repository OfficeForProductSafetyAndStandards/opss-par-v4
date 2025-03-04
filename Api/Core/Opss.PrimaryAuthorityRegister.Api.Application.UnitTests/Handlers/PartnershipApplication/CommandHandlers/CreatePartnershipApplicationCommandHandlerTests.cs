using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opss.PrimaryAuthorityRegister.Cqrs.Requests.PartnershipApplication.Commands;

public class CreatePartnershipApplicationCommandTests
{
    [Fact]
    public void GivenConstructor_WhenPartnershipTypeProvided_ThenPropertiesSet()
    {
        var type = PartnershipConstants.PartnershipType.Direct;
        
        var command = new CreatePartnershipApplicationCommand(type);

        Assert.Equal(type, command.PartnershipType);
    }
}
