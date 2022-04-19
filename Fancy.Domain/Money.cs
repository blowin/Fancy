namespace Fancy.Domain
{
    public record struct Money(decimal Value)
    {
        public static Money Zero => new Money(0);

        public Money Ceiling => new Money(Math.Ceiling(Value));

        public static Money operator -(Money l, Money r) => new Money(l.Value - r.Value);

        public static Money operator +(Money l, Money r) => new Money(l.Value + r.Value);

        public static bool operator >(Money l, Money r) => l.Value > r.Value;

        public static bool operator <(Money l, Money r) => l.Value < r.Value;

        public static bool operator >=(Money l, Money r) => l.Value >= r.Value;

        public static bool operator <=(Money l, Money r) => l.Value <= r.Value;

        public static Money operator*(Money l, decimal r) => new Money(l.Value * r);

        public static Money operator/(decimal l, Money r) => new Money(l / r.Value);

        public static Money operator /(Money l, decimal r) => new Money(l.Value / r);

        public static Money operator *(decimal l, Money r) => new Money(l * r.Value);

        public override string ToString() => Value.ToString();
    }
}
