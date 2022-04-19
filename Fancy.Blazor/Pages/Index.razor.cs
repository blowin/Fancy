using Fancy.Blazor.Expenses;
using Fancy.Blazor.Shared;
using Fancy.Domain.Expenses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Fancy.Blazor.Pages
{
    public partial class Index
    {
        private string? _expenseName;
        private decimal _expenseAmount;
        private RepeatTypeComponent _repeatTypeComponent = null!;
        private List<Expense> _expenses = new List<Expense>();
        private IList<MonthExpense> _monthExpenses = Array.Empty<MonthExpense>();

        [Inject]
        public ExpenseCalculator ExpenseCalculator { get; set; } = null!;

        [Inject]
        public ExpensesStore ExpensesStore { get; set; } = null!;

        [Inject]
        public ExpenseImportExportService ExpenseImportExportService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            _expenses = await ExpensesStore.LoadExpensesAsync();
            Calculate();
        }

        private async ValueTask Add()
        {
            var ex = new Expense(_expenseName ?? string.Empty, _expenseAmount, _repeatTypeComponent.GetRepeatType());
            _expenses.Add(ex);
            await ExpensesStore.SaveExpensesAsync(_expenses);
            Calculate();
        }

        private void Calculate()
        {
            _monthExpenses = ExpenseCalculator.Calculate(_expenses).ToList();
        }

        private async ValueTask Remove(Expense expense)
        {
            _expenses.Remove(expense);
            await ExpensesStore.SaveExpensesAsync(_expenses);
            Calculate();
        }

        private async ValueTask UploadFiles(InputFileChangeEventArgs e)
        {
            var result = await ExpenseImportExportService.LoadAsync(e.File);
            if (result.Count == 0)
                return;

            _expenses.AddRange(result);
            Calculate();
        }
    }
}
