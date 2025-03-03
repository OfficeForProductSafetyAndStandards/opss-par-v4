using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Common.Constants;
using Opss.PrimaryAuthorityRegister.Common.Exceptions;
using System.Collections.ObjectModel;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Extensions;

public class WebApplicationExtensionsTests
{
    [Fact]
    public async Task SeedIdentity_ShouldThrowArgumentNullException_WhenAppIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => WebApplicationExtensions.SeedIdentity(null));
    }

    [Fact]
    public async Task SeedIdentity_ShouldThrowServiceNotFoundException_WhenScopeFactoryIsMissing()
    {
        var _appMock = new Mock<IApplicationBuilder>();

        _appMock.Setup(a => a.ApplicationServices.GetService(typeof(IServiceScopeFactory)))
                .Returns(null);

        await Assert.ThrowsAsync<ServiceNotFoundException>(() => WebApplicationExtensions.SeedIdentity(_appMock.Object));
    }

    [Fact]
    public async Task SeedIdentity_ShouldReturnZero_WhenSeedDataIsNull()
    {
        var configData = new Dictionary<string, string>();

        var config = new ConfigurationBuilder()
           .AddInMemoryCollection(configData)
           .Build();

        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);

        var app = builder.Build();

        var result = await app.SeedIdentity();

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task SeedIdentity_ShouldAddRolesAndUsers_WhenNoRolesExist()
    {
        var configData = new Dictionary<string, string>
        {
            {"SeedData:Identities:0:Email", "richard.priddy@businessandtrade.gov.uk" },
            {"SeedData:Identities:0:Role", IdentityConstants.Roles.AuthorityMember},
            {"SeedData:Identities:1:Email", "scott.wakefield@businessandtrade.gov.uk"},
            {"SeedData:Identities:1:Role", IdentityConstants.Roles.LAU}
        };

        var config = new ConfigurationBuilder()
           .AddInMemoryCollection(configData)
           .Build();

        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRoleRepository = new Mock<IGenericRepository<Role>>();
        mockRoleRepository.Setup(r => r.Local()).Returns(() =>
            new ReadOnlyCollection<Role>(IdentityConstants.Roles.AllRoles.Select(r => new Role(r)).ToList()));
        mockUnitOfWork.Setup(u => u.Repository<Role>()).Returns(mockRoleRepository.Object);
        mockUnitOfWork.Setup(u => u.Repository<UserIdentity>()).Returns(new Mock<IGenericRepository<UserIdentity>>().Object);

        builder.Services.AddTransient((IServiceProvider provider) => mockUnitOfWork.Object);

        builder.Services.AddTransient((IServiceProvider provider) => new Mock<IUserIdentityRepository>().Object);

        var app = builder.Build();

        await app.SeedIdentity();

        mockUnitOfWork.Verify(u => u.Repository<Role>().AddAsync(It.IsAny<Role>()), Times.Exactly(IdentityConstants.Roles.AllRoles.Length));
        mockUnitOfWork.Verify(u => u.Repository<UserIdentity>().AddAsync(It.IsAny<UserIdentity>()), Times.Exactly(2));

        mockUnitOfWork.Verify(u => u.Save(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SeedIdentity_ShouldAddUsers_WhenRolesExist()
    {
        var configData = new Dictionary<string, string>
        {
            {"SeedData:Identities:0:Email", "richard.priddy@businessandtrade.gov.uk" },
            {"SeedData:Identities:0:Role", IdentityConstants.Roles.AuthorityMember},
            {"SeedData:Identities:1:Email", "scott.wakefield@businessandtrade.gov.uk"},
            {"SeedData:Identities:1:Role", IdentityConstants.Roles.LAU}
        };

        var config = new ConfigurationBuilder()
           .AddInMemoryCollection(configData)
           .Build();

        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRoleRepository = new Mock<IGenericRepository<Role>>();
        mockRoleRepository.Setup(r => r.Local()).Returns(() =>
            new ReadOnlyCollection<Role>(IdentityConstants.Roles.AllRoles.Select(r => new Role(r)).ToList()));
        mockRoleRepository.Setup(r => r.Entities).Returns(() =>
            IdentityConstants.Roles.AllRoles.Select(r => new Role(r)).AsQueryable());
        mockUnitOfWork.Setup(u => u.Repository<Role>()).Returns(mockRoleRepository.Object);
        mockUnitOfWork.Setup(u => u.Repository<UserIdentity>()).Returns(new Mock<IGenericRepository<UserIdentity>>().Object);

        builder.Services.AddTransient((IServiceProvider provider) => mockUnitOfWork.Object);

        builder.Services.AddTransient((IServiceProvider provider) => new Mock<IUserIdentityRepository>().Object);

        var app = builder.Build();

        await app.SeedIdentity();

        mockUnitOfWork.Verify(u => u.Repository<Role>().AddAsync(It.IsAny<Role>()), Times.Never);
        mockUnitOfWork.Verify(u => u.Repository<UserIdentity>().AddAsync(It.IsAny<UserIdentity>()), Times.Exactly(2));

        mockUnitOfWork.Verify(u => u.Save(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SeedIdentity_ShouldAddNothing_WhenRolesAndUsersExist()
    {
        var configData = new Dictionary<string, string>
        {
            {"SeedData:Identities:0:Email", "richard.priddy@businessandtrade.gov.uk" },
            {"SeedData:Identities:0:Role", IdentityConstants.Roles.AuthorityMember },
            {"SeedData:Identities:1:Email", "scott.wakefield@businessandtrade.gov.uk"},
            {"SeedData:Identities:1:Role", IdentityConstants.Roles.LAU}
        };

        var config = new ConfigurationBuilder()
           .AddInMemoryCollection(configData)
           .Build();

        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRoleRepository = new Mock<IGenericRepository<Role>>();
        mockRoleRepository.Setup(r => r.Local()).Returns(() =>
            new ReadOnlyCollection<Role>(IdentityConstants.Roles.AllRoles.Select(r => new Role(r)).ToList()));
        mockRoleRepository.Setup(r => r.Entities).Returns(() =>
            IdentityConstants.Roles.AllRoles.Select(r => new Role(r)).AsQueryable());
        mockUnitOfWork.Setup(u => u.Repository<Role>()).Returns(mockRoleRepository.Object);
        mockUnitOfWork.Setup(u => u.Repository<UserIdentity>()).Returns(new Mock<IGenericRepository<UserIdentity>>().Object);

        builder.Services.AddTransient((IServiceProvider provider) => mockUnitOfWork.Object);

        var mockUserIdentityRepository = new Mock<IUserIdentityRepository>();
        mockUserIdentityRepository.Setup(r => r.GetUserIdentiyByEmail(It.IsAny<string>())).Returns(() => new UserIdentity(""));
        builder.Services.AddTransient((IServiceProvider provider) => mockUserIdentityRepository.Object);

        var app = builder.Build();

        await app.SeedIdentity();

        mockUnitOfWork.Verify(u => u.Repository<Role>().AddAsync(It.IsAny<Role>()), Times.Never);
        mockUnitOfWork.Verify(u => u.Repository<UserIdentity>().AddAsync(It.IsAny<UserIdentity>()), Times.Never);

        mockUnitOfWork.Verify(u => u.Save(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SeedIdentity_ShouldAddRegulatoryFunctions_WhenNoneExist()
    {
        var configData = new Dictionary<string, string>
        {
            {"SeedData:RegulatoryFunctions:0", "Environmental Health"},
            {"SeedData:RegulatoryFunctions:1", "Trading Standards"},
            {"SeedData:RegulatoryFunctions:2", "Fire Safety"},
            {"SeedData:RegulatoryFunctions:3", "Licensing"},
            {"SeedData:RegulatoryFunctions:4", "Petrol Storage Certification"},
            {"SeedData:RegulatoryFunctions:5", "Explosives Licensing"},
            {"SeedData:RegulatoryFunctions:6", "Health and Safety (reserved functions only)"},
        };

        var config = new ConfigurationBuilder()
           .AddInMemoryCollection(configData)
           .Build();

        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);

        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var mockRoleRepository = new Mock<IGenericRepository<Role>>();
        mockRoleRepository.Setup(r => r.Local()).Returns(() =>
            new ReadOnlyCollection<Role>(IdentityConstants.Roles.AllRoles.Select(r => new Role(r)).ToList()));
        mockRoleRepository.Setup(r => r.Entities).Returns(() =>
            IdentityConstants.Roles.AllRoles.Select(r => new Role(r)).AsQueryable());
        mockUnitOfWork.Setup(u => u.Repository<Role>()).Returns(mockRoleRepository.Object);

        mockUnitOfWork.Setup(u => u.Repository<UserIdentity>()).Returns(new Mock<IGenericRepository<UserIdentity>>().Object);

        var mockFunctionRepo = new Mock<IGenericRepository<RegulatoryFunction>>();
        mockFunctionRepo.Setup(r => r.Entities).Returns(() =>
            new List<RegulatoryFunction>().AsQueryable());
        mockUnitOfWork.Setup(u => u.Repository<RegulatoryFunction>()).Returns(mockFunctionRepo.Object);

        builder.Services.AddTransient((IServiceProvider provider) => mockUnitOfWork.Object);

        var mockUserIdentityRepository = new Mock<IUserIdentityRepository>();
        mockUserIdentityRepository.Setup(r => r.GetUserIdentiyByEmail(It.IsAny<string>())).Returns(() => null);
        builder.Services.AddTransient((IServiceProvider provider) => mockUserIdentityRepository.Object);

        var app = builder.Build();

        await app.SeedIdentity();

        mockUnitOfWork.Verify(u => u.Repository<RegulatoryFunction>().AddAsync(It.IsAny<RegulatoryFunction>()), Times.Exactly(7));

        mockUnitOfWork.Verify(u => u.Save(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SeedIdentity_ShouldAddAuthorities_WhenNoneExist()
    {
        var configData = new Dictionary<string, string>
        {
            { "SeedData:Authorities:0:Name", "Local Council"},
            { "SeedData:Authorities:0:Identities:0", "user@example.com"},
            { "SeedData:Authorities:0:RegulatoryFunctions:0", "Environmental Health"},
            { "SeedData:Authorities:0:RegulatoryFunctions:1", "Trading Standards"}
        };

        var config = new ConfigurationBuilder()
           .AddInMemoryCollection(configData)
           .Build();

        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);

        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var mockRoleRepository = new Mock<IGenericRepository<Role>>();
        mockRoleRepository.Setup(r => r.Local()).Returns(() =>
            new ReadOnlyCollection<Role>(IdentityConstants.Roles.AllRoles.Select(r => new Role(r)).ToList()));
        mockRoleRepository.Setup(r => r.Entities).Returns(() =>
            IdentityConstants.Roles.AllRoles.Select(r => new Role(r)).AsQueryable());
        mockUnitOfWork.Setup(u => u.Repository<Role>()).Returns(mockRoleRepository.Object);

        mockUnitOfWork.Setup(u => u.Repository<UserIdentity>()).Returns(new Mock<IGenericRepository<UserIdentity>>().Object);

        var functions = new List<string>
        {
            "Environmental Health",
            "Trading Standards",
            "Fire Safety",
            "Licensing",
            "Petrol Storage Certification",
            "Explosives Licensing",
            "Health and Safety (reserved functions only)"
        };
        var mockFunctionRepo = new Mock<IGenericRepository<RegulatoryFunction>>();
        mockFunctionRepo.Setup(r => r.Local()).Returns(() =>
            new ReadOnlyCollection<RegulatoryFunction>(functions.Select(r => new RegulatoryFunction(r)).ToList()));
        mockUnitOfWork.Setup(u => u.Repository<RegulatoryFunction>()).Returns(mockFunctionRepo.Object);

        var mockAuthorityRepo = new Mock<IGenericRepository<Authority>>();
        mockAuthorityRepo.Setup(r => r.Entities).Returns(() => new List<Authority>().AsQueryable());
        mockUnitOfWork.Setup(u => u.Repository<Authority>()).Returns(mockAuthorityRepo.Object);

        builder.Services.AddTransient((IServiceProvider provider) => mockUnitOfWork.Object);

        var mockUserIdentityRepository = new Mock<IUserIdentityRepository>();
        mockUserIdentityRepository.Setup(r => r.GetUserIdentiyByEmail(It.IsAny<string>()))
            .Returns(() => new UserIdentity("user@example.com"));
        builder.Services.AddTransient((IServiceProvider provider) => mockUserIdentityRepository.Object);

        var app = builder.Build();

        await app.SeedIdentity();

        mockUnitOfWork.Verify(u => u.Repository<Authority>().AddAsync(It.IsAny<Authority>()), Times.Once());

        mockUnitOfWork.Verify(u => u.Save(CancellationToken.None), Times.Once);
    }
}
