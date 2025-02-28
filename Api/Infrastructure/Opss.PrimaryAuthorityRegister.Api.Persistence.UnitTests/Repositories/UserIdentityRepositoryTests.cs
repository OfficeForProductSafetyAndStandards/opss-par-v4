using Microsoft.EntityFrameworkCore;
using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Repositories;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.UnitTests.Repositories;

public class UserIdentityRepositoryTests
{
    private readonly Mock<IGenericRepository<UserIdentity>> _repositoryMock;
    private readonly UserIdentityRepository _userIdentityRepository;

    public UserIdentityRepositoryTests()
    {
        _repositoryMock = new Mock<IGenericRepository<UserIdentity>>();
        _userIdentityRepository = new UserIdentityRepository(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetUserIdentityByEmailAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var email = "test@example.com";
        var user = new UserIdentity(email, new Role("Admin"));
        var users = new[] { user }.AsQueryable();

        var mockDbSet = new Mock<DbSet<UserIdentity>>();
        mockDbSet.As<IQueryable<UserIdentity>>().Setup(m => m.Provider).Returns(users.Provider);
        mockDbSet.As<IQueryable<UserIdentity>>().Setup(m => m.Expression).Returns(users.Expression);
        mockDbSet.As<IQueryable<UserIdentity>>().Setup(m => m.ElementType).Returns(users.ElementType);
        mockDbSet.As<IQueryable<UserIdentity>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

        _repositoryMock.Setup(r => r.Entities).Returns(mockDbSet.Object);

        // Act
        var result = _userIdentityRepository.GetUserIdentiyByEmail(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.EmailAddress);
        Assert.Single(result.Roles);
    }

    [Fact]
    public async Task GetUserIdentityByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "notfound@example.com";
        var users = new UserIdentity[] { }.AsQueryable();

        var mockDbSet = new Mock<DbSet<UserIdentity>>();
        mockDbSet.As<IQueryable<UserIdentity>>().Setup(m => m.Provider).Returns(users.Provider);
        mockDbSet.As<IQueryable<UserIdentity>>().Setup(m => m.Expression).Returns(users.Expression);
        mockDbSet.As<IQueryable<UserIdentity>>().Setup(m => m.ElementType).Returns(users.ElementType);
        mockDbSet.As<IQueryable<UserIdentity>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

        _repositoryMock.Setup(r => r.Entities).Returns(mockDbSet.Object);

        // Act
        var result = _userIdentityRepository.GetUserIdentiyByEmail(email);

        // Assert
        Assert.Null(result);
    }
}

