using Opss.PrimaryAuthorityRegister.Common.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opss.PrimaryAuthorityRegister.Common.UnitTests.Providers;

public class DateTimeOverrideProviderTests
{
    private readonly DateTimeOverrideProvider _dateTimeProvider;

    public DateTimeOverrideProviderTests()
    {
        _dateTimeProvider = new DateTimeOverrideProvider();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithCurrentUtcDate()
    {
        // Act
        var today = _dateTimeProvider.UtcDateNow;

        // Assert
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), today);
    }

    [Fact]
    public void OverrideDate_ShouldChangeDate()
    {
        // Arrange
        var newDate = new DateOnly(2025, 01, 01);

        // Act
        _dateTimeProvider.OverrideDate(newDate);

        // Assert
        Assert.Equal(newDate, _dateTimeProvider.UtcDateNow);
    }

    [Theory]
    [InlineData("2025-06-13", "2025-06-14", DateRange.Inclusive, false)]
    [InlineData("2025-06-14", "2025-06-15", DateRange.Inclusive, true)]
    [InlineData("2025-06-15", "2025-06-16", DateRange.Inclusive, true)]
    [InlineData("2025-06-16", "2025-06-17", DateRange.Inclusive, false)]
    [InlineData("2025-06-14", "2025-06-16", DateRange.Inclusive, true)]
    [InlineData("2025-06-13", "2025-06-14", DateRange.Exclusive, false)]
    [InlineData("2025-06-14", "2025-06-15", DateRange.Exclusive, false)]
    [InlineData("2025-06-15", "2025-06-16", DateRange.Exclusive, false)]
    [InlineData("2025-06-16", "2025-06-17", DateRange.Exclusive, false)]
    [InlineData("2025-06-14", "2025-06-16", DateRange.Exclusive, true)]
    [InlineData("2025-06-13", "2025-06-14", DateRange.IncludeEnd, false)]
    [InlineData("2025-06-14", "2025-06-15", DateRange.IncludeEnd, true)]
    [InlineData("2025-06-15", "2025-06-16", DateRange.IncludeEnd, false)]
    [InlineData("2025-06-16", "2025-06-17", DateRange.IncludeEnd, false)]
    [InlineData("2025-06-14", "2025-06-16", DateRange.IncludeEnd, true)]
    [InlineData("2025-06-13", "2025-06-14", DateRange.IncludeStart, false)]
    [InlineData("2025-06-14", "2025-06-15", DateRange.IncludeStart, false)]
    [InlineData("2025-06-15", "2025-06-16", DateRange.IncludeStart, true)]
    [InlineData("2025-06-16", "2025-06-17", DateRange.IncludeStart, false)]
    [InlineData("2025-06-14", "2025-06-16", DateRange.IncludeStart, true)]
    public void IsNowWithinRange_ShouldReturnCorrectResult(string start, string end, DateRange range, bool expected)
    {
        // Arrange
        var startDate = DateOnly.Parse(start);
        var endDate = DateOnly.Parse(end);
        _dateTimeProvider.OverrideDate(new DateOnly(2025, 06, 15));

        // Act
        var result = _dateTimeProvider.IsNowWithinRange(startDate, endDate, range);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("2025-06-13", "2025-06-14", DateRange.Inclusive, false)]
    [InlineData("2025-06-14", "2025-06-15", DateRange.Inclusive, true)]
    [InlineData("2025-06-15", "2025-06-16", DateRange.Inclusive, true)]
    [InlineData("2025-06-16", "2025-06-17", DateRange.Inclusive, false)]
    [InlineData("2025-06-14", "2025-06-16", DateRange.Inclusive, true)]
    [InlineData("2025-06-13", "2025-06-14", DateRange.Exclusive, false)]
    [InlineData("2025-06-14", "2025-06-15", DateRange.Exclusive, false)]
    [InlineData("2025-06-15", "2025-06-16", DateRange.Exclusive, false)]
    [InlineData("2025-06-16", "2025-06-17", DateRange.Exclusive, false)]
    [InlineData("2025-06-14", "2025-06-16", DateRange.Exclusive, true)]
    [InlineData("2025-06-13", "2025-06-14", DateRange.IncludeEnd, false)]
    [InlineData("2025-06-14", "2025-06-15", DateRange.IncludeEnd, true)]
    [InlineData("2025-06-15", "2025-06-16", DateRange.IncludeEnd, false)]
    [InlineData("2025-06-16", "2025-06-17", DateRange.IncludeEnd, false)]
    [InlineData("2025-06-14", "2025-06-16", DateRange.IncludeEnd, true)]
    [InlineData("2025-06-13", "2025-06-14", DateRange.IncludeStart, false)]
    [InlineData("2025-06-14", "2025-06-15", DateRange.IncludeStart, false)]
    [InlineData("2025-06-15", "2025-06-16", DateRange.IncludeStart, true)]
    [InlineData("2025-06-16", "2025-06-17", DateRange.IncludeStart, false)]
    [InlineData("2025-06-14", "2025-06-16", DateRange.IncludeStart, true)]
    public void IsWithinRange_ShouldReturnCorrectResult(string start, string end, DateRange range, bool expected)
    {
        // Arrange
        var startDate = DateOnly.Parse(start);
        var endDate = DateOnly.Parse(end);
        var now = new DateOnly(2025, 06, 15);

        // Act
        var result = _dateTimeProvider.IsWithinRange(now, startDate, endDate, range);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UtcNow_ShouldMaintainCurrentTime()
    {
        // Arrange
        _dateTimeProvider.OverrideDate(new DateOnly(2025, 06, 15));
        var expectedTime = DateTime.UtcNow;

        // Act
        var utcNow = _dateTimeProvider.UtcNow;

        // Assert
        Assert.Equal(2025, utcNow.Year);
        Assert.Equal(6, utcNow.Month);
        Assert.Equal(15, utcNow.Day);
        Assert.Equal(expectedTime.Hour, utcNow.Hour);
        Assert.Equal(expectedTime.Minute, utcNow.Minute);
    }
}

