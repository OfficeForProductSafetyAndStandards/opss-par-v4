namespace Opss.PrimaryAuthorityRegister.Common.Providers;

/// <summary>
/// A special DateTime provider allowing testers to override the current date
/// </summary>
public class DateTimeOverrideProvider : IDateTimeProvider
{
    private DateOnly Date { get; set; }

    public DateTimeOverrideProvider()
    {
        Date = DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public void OverrideDate(DateOnly date)
    {
        Date = date;
    }

    public bool IsNowWithinRange(DateOnly startDate, DateOnly endDate, DateRange range = DateRange.Inclusive)
    {
        switch (range)
        {
            case DateRange.Inclusive:
                return Date >= startDate && Date <= endDate;
            case DateRange.Exclusive:
                return Date > startDate && Date < endDate;
            case DateRange.IncludeEnd:
                return Date > startDate && Date <= endDate;
            case DateRange.IncludeStart:
                return Date >= startDate && Date < endDate;
        }
        return Date >= startDate && Date <= endDate;
    }

    public bool IsWithinRange(DateOnly date, DateOnly startDate, DateOnly endDate, DateRange range = DateRange.Inclusive)
    {
        switch (range)
        {
            case DateRange.Inclusive:
                return date >= startDate && date <= endDate;
            case DateRange.Exclusive:
                return date > startDate && date < endDate;
            case DateRange.IncludeEnd:
                return date > startDate && date <= endDate;
            case DateRange.IncludeStart:
                return date >= startDate && date < endDate;
        }
        return date >= startDate && date <= endDate;
    }

    public DateTime UtcNow
    {
        get
        {
            // Get the current date and time in UTC
            DateTime currentUtcDateTime = DateTime.UtcNow;

            // Set the specified date while maintaining the current time
            DateTime specificDateTimeWithCurrentTime = new DateTime(
                Date.Year, Date.Month, Date.Day,
                currentUtcDateTime.Hour, currentUtcDateTime.Minute, currentUtcDateTime.Second,
                currentUtcDateTime.Millisecond, DateTimeKind.Utc
            );

            return specificDateTimeWithCurrentTime;
        }
    }

    public DateOnly UtcDateNow
    {
        get
        {
            var (date, _) = UtcNow;
            return date;
        }
    }
}