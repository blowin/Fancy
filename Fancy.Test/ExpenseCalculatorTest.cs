using System;
using System.Collections.Generic;
using System.Linq;
using Fancy.Domain;
using Fancy.Domain.Expenses;
using Xunit;

namespace Fancy.Test;

public class ExpenseCalculatorTest
{
    [Theory]
    [MemberData(nameof(Data.Expenses), MemberType = typeof(Data))]
    public void Calculate(List<Expense> expenses, SafeBagPlan safeBagPlan, int currentAmountOfMoneyInSafeBag, List<MonthExpense> expectExpenses)
    {
        var calculator = new ExpenseCalculator();
        
        var actualExpenses = calculator.CalculateWithSafetyBag(expenses, safeBagPlan, currentAmountOfMoneyInSafeBag).ToList();

        Assert.Equal(expectExpenses.Count, actualExpenses.Count);
        Assert.Equal(expectExpenses, actualExpenses);
    }
    
    public class Data
    {
        public static IEnumerable<object> Expenses
        {
            get
            {
                yield return new object[]
                {
                    new List<Expense>
                    {
                        new("Еда", 300, MultipleInMonthRepeatType.Once),
                        new("Квартира", 400, MultipleInMonthRepeatType.Once),
                        new("Комуналка", 100, MultipleInMonthRepeatType.Once),

                        new("Отпуск", 3000, MultipleInYearRepeatType.Once),

                        new("Покупка квартиры", 100_000, new DurationYearRepeatType(2.5m)),
                        new("Покупка машины", 35_000, new DurationYearRepeatType(2.5m)),
                    },
                    new SafeBagPlan(TimeSpan.FromDays(183)),
                    6000,
                    
                    new List<MonthExpense>
                    {
                        new("Еда", 300),
                        new("Квартира", 400),
                        new("Комуналка", 100),

                        new("Отпуск", 250),

                        new("Покупка квартиры", 3334),
                        new("Покупка машины", 1167),
                    }
                };
                
                yield return new object[]
                {
                    new List<Expense> {},
                    new SafeBagPlan(TimeSpan.FromDays(183)),
                    6000,
                    new List<MonthExpense>{}
                };
                
                yield return new object[]
                {
                    new List<Expense>{ new("Еда", 300, MultipleInMonthRepeatType.Once) },
                    new SafeBagPlan(TimeSpan.FromDays(183)),
                    1800,
                    new List<MonthExpense> { new("Еда", 300) }
                };
                
                yield return new object[]
                {
                    new List<Expense> { new("Еда", 300, MultipleInMonthRepeatType.Once), },
                    new SafeBagPlan(TimeSpan.FromDays(183)),
                    1799,
                    new List<MonthExpense> { new("Еда", 300), new(SafeBagPlan.FundraisingName, 1), }
                };
            }
        }
    }
}