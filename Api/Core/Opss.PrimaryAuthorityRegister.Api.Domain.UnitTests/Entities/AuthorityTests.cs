using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.UnitTests.Entities;

public class AuthorityTests
{
    [Fact]
    public void GivenName_ThenPropertiesPopulated()
    {
        var name = "Authority";
        var authority = new Authority(name);

        Assert.Equal(name, authority.Name);
    }

    [Fact]
    public void GivenNameAndFunctions_ThenPropertiesPopulated()
    {
        var name = "Authority";
        var functionName = "Function";
        var function = new RegulatoryFunction(functionName);
        var authority = new Authority(name, [function]);

        Assert.Equal(name, authority.Name);
        Assert.Contains(function, authority.RegulatoryFunctions);
    }

    [Fact]
    public void GivenAuthority_WhenAdddingUser_ThenUserAdded()
    {
        var name = "Authority";
        var authority = new Authority(name);

        var user = new UserIdentity("user@example.com");

        authority.AddUser(user);

        Assert.Contains(user.Id, authority.AuthorityUsers.Select(r => r.UserIdentityId));
    }

    [Fact]
    public void GivenAuthority_WhenAddingNullUser_ThenExceptionThrown()
    {
        var name = "Authority";
        var authority = new Authority(name);

        Assert.Throws<ArgumentNullException>(() => authority.AddUser(null));
    }

    [Fact]
    public void GivenAuthority_WhenAdddingRegulatoryFunction_ThenRegulatoryFunctionAdded()
    {
        var name = "Authority";
        var authority = new Authority(name);

        var function = new RegulatoryFunction("Function");

        authority.AddRegulatoryFunction(function);

        Assert.Contains(function, authority.RegulatoryFunctions);
    }

    [Fact]
    public void GivenAuthority_WhenAddingNullRegulatoryFunction_ThenExceptionThrown()
    {
        var name = "Authority";
        var authority = new Authority(name);

        Assert.Throws<ArgumentNullException>(() => authority.AddRegulatoryFunction(null));
    }

    [Fact]
    public void GivenAuthority_WhenAddingPartnershipApplication_ThenPartnershipApplicationAdded()
    {
        var name = "Authority";
        var authority = new Authority(name);

        var function = new PartnershipApplication();

        authority.AddPartnershipApplication(function);

        Assert.Contains(function, authority.PartnershipApplications);
    }

    [Fact]
    public void GivenAuthority_WhenAddingNullPartnershipApplication_ThenArgumentNullExceptionThrown()
    {
        var name = "Authority";
        var authority = new Authority(name);

        Assert.Throws<ArgumentNullException>(() => authority.AddPartnershipApplication(null));
    }
}
