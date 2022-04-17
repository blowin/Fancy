namespace Fancy.Domain;

public abstract class RepeatType : IEquatable<RepeatType>
{
    public abstract string Name { get; }

    public abstract decimal MonthlyAmount(decimal amount);

    public override string ToString() => Name;
    
    public abstract bool Equals(RepeatType? other);

#pragma warning disable CS0659
    public override bool Equals(object? obj)
#pragma warning restore CS0659
    {
        if (ReferenceEquals(null, obj)) 
            return false;
        
        if (ReferenceEquals(this, obj)) 
            return true;
        
        if (obj.GetType() != this.GetType()) 
            return false;
        
        return Equals((RepeatType)obj);
    }
}

public sealed class MultipleInYearRepeatType : RepeatType
{
    public static readonly RepeatType Once = new MultipleInYearRepeatType(1);
    
    public MultipleInYearRepeatType(int repeatTimes) => RepeatTimes = repeatTimes;

    public int RepeatTimes { get; }

    public override string Name => $"{RepeatTimes} раз в год";

    public override decimal MonthlyAmount(decimal amount) => (amount * RepeatTimes) / 12.0m;
    
    public override bool Equals(RepeatType? other) => other is MultipleInYearRepeatType type && type.RepeatTimes == RepeatTimes;

    public override int GetHashCode() => HashCode.Combine(GetType(), RepeatTimes);
}

public sealed class MultipleInMonthRepeatType : RepeatType
{
    public static readonly RepeatType Once = new MultipleInMonthRepeatType(1);
    
    public MultipleInMonthRepeatType(int repeatTimes) => RepeatTimes = repeatTimes;

    public int RepeatTimes { get; }

    public override string Name => $"{RepeatTimes} раз в месяц";

    public override decimal MonthlyAmount(decimal amount) => amount * RepeatTimes;
    
    public override bool Equals(RepeatType? other) => other is MultipleInMonthRepeatType type && type.RepeatTimes == RepeatTimes;

    public override int GetHashCode() => HashCode.Combine(GetType(), RepeatTimes);
}

public sealed class DurationYearRepeatType : RepeatType
{
    public DurationYearRepeatType(decimal duration) => Duration = Math.Ceiling(duration);

    public override string Name => $"Через {Duration:F1} лет";
    
    public decimal Duration { get; }

    public override decimal MonthlyAmount(decimal amount) => amount / (12.0m * Duration);

    public override bool Equals(RepeatType? other) => other is DurationYearRepeatType type && type.Duration == Duration;

    public override int GetHashCode() => HashCode.Combine(GetType(), Duration);
}