using Fancy.Domain;
using Fancy.Domain.Expenses;

namespace Fancy.Blazor.Expenses
{
    public sealed class SaveExpense
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }

        public decimal RepeatTypeTimes { get; set; }
        public SaveRepeatTypeType RepeatType { get; set; }

        public SaveExpense(Expense expense)
        {
            Name = expense.Name;
            Amount = expense.Amount.Value;

            (RepeatTypeTimes, RepeatType) = expense.RepeatType.Match(yr => ((decimal)yr.RepeatTimes, SaveRepeatTypeType.MultipleInYear),
                month => ((decimal)month.RepeatTimes, SaveRepeatTypeType.MultipleInMonth),
                duration => (duration.Duration, SaveRepeatTypeType.DurationYear));
        }

        public SaveExpense()
        {
            Name = String.Empty;
        }

        public Expense ToExpense() => new Expense(Name, new Money(Amount), GetRepeatType());

        private RepeatType GetRepeatType()
        {
            switch (RepeatType)
            {
                case SaveRepeatTypeType.MultipleInMonth:
                    return new MultipleInMonthRepeatType((int)RepeatTypeTimes);
                case SaveRepeatTypeType.MultipleInYear:
                    return new MultipleInYearRepeatType((int)RepeatTypeTimes);
                case SaveRepeatTypeType.DurationYear:
                    return new DurationYearRepeatType(RepeatTypeTimes);
            }

            throw new InvalidOperationException();
        }
    }
}
