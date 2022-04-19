namespace Fancy.Domain.Expenses;

public class ExpenseCalculator
{
    public IEnumerable<MonthExpense> CalculateWithSafetyBag(ICollection<Expense> expenses, SafeBagPlan safeBagPlan, Money currentAmountOfMoneyInSafeBag = default)
    {
        var safeBagExpense = safeBagPlan.ToExpense(expenses, currentAmountOfMoneyInSafeBag);
        if (safeBagExpense == null)
            return Calculate(expenses);
        return Calculate(new List<Expense>(expenses) { safeBagExpense });
    }
    
    public IEnumerable<MonthExpense> Calculate(IEnumerable<Expense> expenses)
    {
        foreach (var (name, amount, repeatType) in expenses)
        {
            var monthlyAmount = repeatType.MonthlyAmount(amount);
            yield return new MonthExpense(name, monthlyAmount.Ceiling);
        }
    }

    public Money Sum(ICollection<Expense> expenses)
    {
        var monthExpenses = Calculate(expenses);
        return Sum(monthExpenses);
    }
    
    public Money Sum(IEnumerable<MonthExpense> expenses)
    {
        var sum = expenses.Select(e => e.Amount.Value).DefaultIfEmpty(0).Sum();
        return new Money(sum);
    }
}