namespace Fancy.Domain.Expenses;

public record Expense(string Name, decimal Amount, RepeatType RepeatType);