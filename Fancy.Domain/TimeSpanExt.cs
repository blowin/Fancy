namespace Fancy.Domain;

public static class TimeSpanExt
{
    public static RepeatType ToRepeatType(this TimeSpan self)
    {
        var years = (decimal)self.TotalMilliseconds / (decimal)(TimeSpan.FromDays(365).TotalMilliseconds);
        return new DurationYearRepeatType(years);
    }
}