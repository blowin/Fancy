namespace Fancy.Domain.Expenses;

public record Expense(string Name, Money Amount, RepeatType RepeatType);