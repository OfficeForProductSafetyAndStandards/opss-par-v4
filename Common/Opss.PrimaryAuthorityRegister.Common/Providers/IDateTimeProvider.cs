namespace Opss.PrimaryAuthorityRegister.Common.Providers;

/// <summary>
/// Provides access to DateTime.Now
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly UtcDateNow { get; }

    void OverrideDate(DateOnly date);
    bool IsNowWithinRange(DateOnly startDate, DateOnly endDate, DateRange range = DateRange.Inclusive);
    bool IsWithinRange(DateOnly date, DateOnly startDate, DateOnly endDate, DateRange range = DateRange.Inclusive);
}
