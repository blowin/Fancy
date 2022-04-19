using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fancy.Blazor;
using Fancy.Domain.Expenses;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Fancy.Blazor.Expenses;
using BlazorDownloadFile;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});
builder.Services.AddMudServices();

builder.Services.AddBlazorDownloadFile();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ExpensesStore>();
builder.Services.AddScoped<ExpenseImportExportService>();

builder.Services.AddSingleton<ExpenseCalculator>();

await builder.Build().RunAsync();