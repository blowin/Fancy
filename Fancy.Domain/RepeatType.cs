using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fancy.Domain;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public abstract class RepeatType : IEquatable<RepeatType>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
{
    public abstract string Name { get; }

    public abstract Money MonthlyAmount(Money amount);

    public abstract TRes Match<TRes>(Func<MultipleInYearRepeatType, TRes> multipleInYearRepeatType,
        Func<MultipleInMonthRepeatType, TRes> multipleInMonthRepeatType,
        Func<DurationYearRepeatType, TRes> durationYearRepeatType);

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

    public override string ToString() => Name;
}

public sealed class MultipleInYearRepeatType : RepeatType
{
    public static readonly RepeatType Once = new MultipleInYearRepeatType(1);
    
    public MultipleInYearRepeatType(int repeatTimes) => RepeatTimes = repeatTimes;

    public int RepeatTimes { get; }

    public override string Name => $"{RepeatTimes} раз в год";

    public override Money MonthlyAmount(Money amount) => (amount * RepeatTimes) / 12.0m;
    
    public override bool Equals(RepeatType? other) => other is MultipleInYearRepeatType type && type.RepeatTimes == RepeatTimes;

    public override int GetHashCode() => HashCode.Combine(GetType(), RepeatTimes);

    public override TRes Match<TRes>(Func<MultipleInYearRepeatType, TRes> multipleInYearRepeatType, 
        Func<MultipleInMonthRepeatType, TRes> multipleInMonthRepeatType, 
        Func<DurationYearRepeatType, TRes> durationYearRepeatType)
    {
        return multipleInYearRepeatType(this);
    }
}

public sealed class MultipleInMonthRepeatType : RepeatType
{
    public static readonly RepeatType Once = new MultipleInMonthRepeatType(1);
    
    public MultipleInMonthRepeatType(int repeatTimes) => RepeatTimes = repeatTimes;
    
    public int RepeatTimes { get; }

    public override string Name => $"{RepeatTimes} раз в месяц";

    public override Money MonthlyAmount(Money amount) => amount * RepeatTimes;
    
    public override bool Equals(RepeatType? other) => other is MultipleInMonthRepeatType type && type.RepeatTimes == RepeatTimes;

    public override int GetHashCode() => HashCode.Combine(GetType(), RepeatTimes);

    public override TRes Match<TRes>(Func<MultipleInYearRepeatType, TRes> multipleInYearRepeatType, 
        Func<MultipleInMonthRepeatType, TRes> multipleInMonthRepeatType, 
        Func<DurationYearRepeatType, TRes> durationYearRepeatType)
    {
        return multipleInMonthRepeatType(this);
    }
}

public sealed class DurationYearRepeatType : RepeatType
{
    public DurationYearRepeatType(decimal duration) => Duration = duration;
    
    public override string Name => $"Через {Duration:0.#} лет";
    
    public decimal Duration { get; }

    public override Money MonthlyAmount(Money amount)
    {
        var calculateMoney = amount / (12.0m * Duration);
        return calculateMoney.Ceiling;
    }

    public override bool Equals(RepeatType? other) => other is DurationYearRepeatType type && type.Duration == Duration;

    public override int GetHashCode() => HashCode.Combine(GetType(), Duration);

    public override TRes Match<TRes>(Func<MultipleInYearRepeatType, TRes> multipleInYearRepeatType, 
        Func<MultipleInMonthRepeatType, TRes> multipleInMonthRepeatType, 
        Func<DurationYearRepeatType, TRes> durationYearRepeatType)
    {
        return durationYearRepeatType(this);
    }
}
