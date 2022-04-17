namespace Fancy.Domain.Expenses;

public class ExpenseCalculator
{
    public IEnumerable<MonthExpense> CalculateWithSafetyBag(ICollection<Expense> expenses, SafeBagPlan safeBagPlan, decimal currentAmountOfMoneyInSafeBag = 0)
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
            yield return new MonthExpense(name, Math.Ceiling(monthlyAmount));
        }
    }

    public decimal Sum(ICollection<Expense> expenses)
    {
        var monthExpenses = Calculate(expenses);
        return Sum(monthExpenses);
    }
    
    public decimal Sum(IEnumerable<MonthExpense> expenses) 
        => expenses.Select(e => e.Amount).DefaultIfEmpty(0).Sum();
}