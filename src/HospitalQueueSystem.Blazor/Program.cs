using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using HospitalQueueSystem.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// ✅ FORMA CORRECTA: Registrar HttpClient con nombre específico
builder.Services.AddHttpClient("HospitalAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7000/");
    client.Timeout = TimeSpan.FromSeconds(30);
    Console.WriteLine("✅ HttpClient configurado con BaseAddress: https://localhost:7000/");
});

// ✅ Registrar servicios que usarán HttpClient
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TurnosService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Forzar puerto 7001
app.Urls.Clear();
app.Urls.Add("https://localhost:7001");
app.Urls.Add("http://localhost:5001");

Console.WriteLine("🚀 Blazor configurado para puerto 7001");

app.Run();