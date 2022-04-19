using Blazored.LocalStorage;
using Fancy.Domain.Expenses;

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
    }
}
