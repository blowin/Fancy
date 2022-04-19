using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using BlazorDownloadFile;
using Fancy.Domain.Expenses;
using Microsoft.AspNetCore.Components.Forms;

namespace Fancy.Blazor.Expenses
{
    public sealed class ExpenseImportExportService
    {
        private const string ContentType = "application/json";

        private static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        private IBlazorDownloadFileService _downloadFileService;

        public ExpenseImportExportService(IBlazorDownloadFileService downloadFileService)
        {
            _downloadFileService = downloadFileService;
        }

        public async ValueTask<DownloadFileResult> ExportAsync(IEnumerable<Expense> expenses, string? fileName = null, CancellationToken cancellationToken = default)
        {
            using var stream = new MemoryStream();

            var mapItems = expenses.Select(x => new SaveExpense(x));
            
            await JsonSerializer.SerializeAsync(stream, mapItems, JsonSerializerOptions, cancellationToken: cancellationToken);
            
            var res = await _downloadFileService.DownloadFile(fileName ?? GenerateFileName(), stream, cancellationToken, cancellationToken, ContentType);
            
            return res;
        }

        public async ValueTask<List<Expense>> LoadAsync(IBrowserFile file, CancellationToken cancellationToken = default)
        {
            try
            {
                if(file.ContentType != ContentType)
                    return new List<Expense>();

                var stream = file.OpenReadStream();
                var result = await JsonSerializer.DeserializeAsync<List<SaveExpense>>(stream, JsonSerializerOptions, cancellationToken: cancellationToken);
                if(result == null)
                    return new List<Expense>();

                return result.Select(x => x.ToExpense()).ToList();
            }
            catch
            {
                return new List<Expense>();
            }
        }

        private static string GenerateFileName()
        {
            var fileName = "expenses_" + DateTime.Now.ToString("dd.MM.yyyy");
            return fileName + ".json";
        }
    }
}
