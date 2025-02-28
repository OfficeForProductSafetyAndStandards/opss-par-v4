using Opss.PrimaryAuthorityRegister.Common.Providers;

namespace Opss.PrimaryAuthorityRegister.Common.UnitTests.Providers;

public class DateTimeProviderTests
{
    private readonly DateTimeProvider _dateTimeProvider;

    public DateTimeProviderTests()
    {
        _dateTimeProvider = new DateTimeProvider();
    }

    [Fact]
    public void UtcNow_ShouldReturnCurrentUtcDateTime()
    {
        // Act
        var now = _dateTimeProvider.UtcNow;

        // Assert
        Assert.True((DateTime.UtcNow - now).TotalSeconds < 1);
    }

    [Fact]
    public void UtcDateNow_ShouldReturnCurrentUtcDate()
    {
        // Act
        var today = _dateTimeProvider.UtcDateNow;

        // Assert
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), today);
    }

    [Theory]
    [InlineData(-2, -1, DateRange.Inclusive, false)]
    [InlineData(-1, 0 , DateRange.Inclusive, true)]
    [InlineData(0 , 1 , DateRange.Inclusive, true)]
    [InlineData(1 , 2 , DateRange.Inclusive, false)]
    [InlineData(-1, 1 , DateRange.Inclusive, true)]
    [InlineData(-2, -1, DateRange.Exclusive, false)]
    [InlineData(-1, 0 , DateRange.Exclusive, false)]
    [InlineData(0 , 1 , DateRange.Exclusive, false)]
    [InlineData(1 , 2 , DateRange.Exclusive, false)]
    [InlineData(-1, 1, DateRange.Exclusive, true)]
    [InlineData(-2, -1, DateRange.IncludeEnd, false)]
    [InlineData(-1, 0 , DateRange.IncludeEnd, true)]
    [InlineData(0 , 1 , DateRange.IncludeEnd, false)]
    [InlineData(1 , 2 , DateRange.IncludeEnd, false)]
    [InlineData(-1, 1, DateRange.IncludeEnd, true)]
    [InlineData(-2, -1, DateRange.IncludeStart, false)]
    [InlineData(-1, 0 , DateRange.IncludeStart, false)]
    [InlineData(0 , 1 , DateRange.IncludeStart, true)]
    [InlineData(1 , 2 , DateRange.IncludeStart, false)]
    [InlineData(-1, 1 , DateRange.IncludeStart, true)]
    public void IsNowWithinRange_ShouldReturnCorrectResult(int startOffset, int endOffset, DateRange range, bool expected)
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(startOffset));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(endOffset));

        // Act
        var result = _dateTimeProvider.IsNowWithinRange(startDate, endDate, range);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(-2, -1, DateRange.Inclusive, false)]
    [InlineData(-1, 0, DateRange.Inclusive, true)]
    [InlineData(0, 1, DateRange.Inclusive, true)]
    [InlineData(1, 2, DateRange.Inclusive, false)]
    [InlineData(-1, 1, DateRange.Inclusive, true)]
    [InlineData(-2, -1, DateRange.Exclusive, false)]
    [InlineData(-1, 0, DateRange.Exclusive, false)]
    [InlineData(0, 1, DateRange.Exclusive, false)]
    [InlineData(1, 2, DateRange.Exclusive, false)]
    [InlineData(-1, 1, DateRange.Exclusive, true)]
    [InlineData(-2, -1, DateRange.IncludeEnd, false)]
    [InlineData(-1, 0, DateRange.IncludeEnd, true)]
    [InlineData(0, 1, DateRange.IncludeEnd, false)]
    [InlineData(1, 2, DateRange.IncludeEnd, false)]
    [InlineData(-1, 1, DateRange.IncludeEnd, true)]
    [InlineData(-2, -1, DateRange.IncludeStart, false)]
    [InlineData(-1, 0, DateRange.IncludeStart, false)]
    [InlineData(0, 1, DateRange.IncludeStart, true)]
    [InlineData(1, 2, DateRange.IncludeStart, false)]
    [InlineData(-1, 1, DateRange.IncludeStart, true)]
    public void IsWithinRange_ShouldReturnCorrectResult(int startOffset, int endOffset, DateRange range, bool expected)
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(startOffset));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(endOffset));
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        // Act
        var result = _dateTimeProvider.IsWithinRange(now, startDate, endDate, range);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void OverrideDate_ShouldNotChangeBehavior()
    {
        // Act
        _dateTimeProvider.OverrideDate(DateOnly.FromDateTime(DateTime.UtcNow));

        // Assert
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), _dateTimeProvider.UtcDateNow);
    }
}
