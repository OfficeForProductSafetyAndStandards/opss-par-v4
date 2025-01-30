using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation.ResourceKeyExpanders;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ResourceKeyExpanders;

public class TestDataResourceKeyExpanderTests
{
    [Fact]
    public void When_ResoureceKey_IsNot_TestData_EmptyArray_IsReturned()
    {
        // Arrange
        var uow = new Mock<IUnitOfWork>().Object;
        var expander = new TestDataResourceKeyExpander(uow);

        // Act
        var expanded = expander.GetKeys("Something");

        // Assert
        Assert.Empty(expanded);
    }
    [Fact]
    public void When_ResoureceKey_Is_TestData_Without_Id_EmptyArray_IsReturned()
    {
        // Arrange
        var uow = new Mock<IUnitOfWork>().Object;
        var expander = new TestDataResourceKeyExpander(uow);

        // Act
        var expanded = expander.GetKeys("TestData/Something");

        // Assert
        Assert.Empty(expanded);
    }

    [Fact]
    public void When_ResourceKey_Is_TestData_ResourceKey_IsExpanded()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        var expectedItem = new TestData(ownerId, "Data");

        var mockRepository = new Mock<IGenericRepository<TestData>>();
        mockRepository
            .Setup(repo => repo.GetByIdAsync(expectedItem.Id))
            .ReturnsAsync(expectedItem);

        var uow = new Mock<IUnitOfWork>();
        uow
            .Setup(i => i.Repository<TestData>())
            .Returns(() => mockRepository.Object);

        var expander = new TestDataResourceKeyExpander(uow.Object);

        // Act
        var expanded = expander.GetKeys($"TestData/{expectedItem.Id}");

        // Assert
        var expectedResults = new[] { $"TestData/Owner/{ownerId}" };

        Assert.Equal(expectedResults, expanded);
    }
}
