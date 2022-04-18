using Blazored.LocalStorage;
using Fancy.Domain;
using Fancy.Domain.Expenses;
using System.Diagnostics;

namespace Fancy.Blazor.Expenses
{
    public class ExpensesStore
    {
        private const string ExpensesListKey = "_Expense_List_Key_";

        private readonly ILocalStorageService _localStorage;

        public ExpensesStore(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public ValueTask SaveExpensesAsync(List<Expense> expenses)
        {
            var ex = expenses.Select(e => new SaveExpense(e)).ToList();
            return _localStorage.SetItemAsync(ExpensesListKey, ex);
        }

        public async ValueTask<List<Expense>> LoadExpensesAsync()
        {
            try
            {
                var items = await _localStorage.GetItemAsync<List<SaveExpense>>(ExpensesListKey);
                if (items == null)
                    return new();

                return items.Select(i => i.ToExpense()).ToList();
            }
            catch
            {
                try 
                { 
                    await _localStorage.RemoveItemAsync(ExpensesListKey); 
                } 
                catch
                {
                    // ignore
                }
                
                return new List<Expense>();
            }
        }

        private class SaveExpense
        {
            public string Name { get; set; }
            public decimal Amount { get; set; }
            
            public decimal RepeatTypeTimes { get; set; }
            public RepeatTypeType RepeatType { get; set; }

            public SaveExpense(Expense expense)
            {
                Name = expense.Name;
                Amount = expense.Amount;

                (RepeatTypeTimes, RepeatType) = expense.RepeatType.Match(yr => ((decimal)yr.RepeatTimes, RepeatTypeType.MultipleInYear),
                    month => ((decimal)month.RepeatTimes, RepeatTypeType.MultipleInMonth),
                    duration => (duration.Duration, RepeatTypeType.DurationYear));
            }

            public SaveExpense() 
            {
                Name = String.Empty;
            }

            public Expense ToExpense() => new Expense(Name, Amount, GetRepeatType());

            private RepeatType GetRepeatType()
            {
                switch (RepeatType)
                {
                    case RepeatTypeType.MultipleInMonth:
                        return new MultipleInMonthRepeatType((int)RepeatTypeTimes);
                    case RepeatTypeType.MultipleInYear:
                        return new MultipleInYearRepeatType((int)RepeatTypeTimes);
                    case RepeatTypeType.DurationYear:
                        return new DurationYearRepeatType(RepeatTypeTimes);
                }

                throw new InvalidOperationException();
            }
        }

        private enum RepeatTypeType : byte
        {
            MultipleInMonth,
            MultipleInYear,
            DurationYear
        }
    }
}
