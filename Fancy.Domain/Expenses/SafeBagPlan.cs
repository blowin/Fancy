namespace Fancy.Domain.Expenses;

/// <param name="FundraisingDeadline">Время за которое хотелось бы собрать подушку</param>
/// <param name="MonthNumber">На какое количество месяцев должна расчитываться подушка</param>
public record SafeBagPlan(TimeSpan FundraisingDeadline, int MonthNumber = 6)
{
    public const string FundraisingName = "Денежная подушка безопасности";
    
    public Expense? ToExpense(IEnumerable<Expense> allExpenses, decimal currentAmountOfMoneyInSafeBag = 0)
    {
        var allMonthExpenses = allExpenses
            .Where(e => e.RepeatType.Equals(MultipleInMonthRepeatType.Once))
            .Select(e => e.Amount)
            .DefaultIfEmpty(0)
            .Sum();
        
        var amount = (allMonthExpenses * MonthNumber);
        if (amount <= currentAmountOfMoneyInSafeBag)
            return null;
        
        return new Expense(FundraisingName, amount - currentAmountOfMoneyInSafeBag, FundraisingDeadline.ToRepeatType());
    }
}