using Stock3DWeb.Data;
using Stock3DWeb.Components;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Db>();
builder.Services.AddScoped<FilamentoRepository>();
builder.Services.AddScoped<ProductoRepository>();
builder.Services.AddScoped<VentaRepository>();

// 1. Leer la cadena de conexión desde appsettings.json (o variable de entorno en la nube)
// Si existe en appsettings.json, sobrescribe la cadena por defecto de DatabaseConfig.
// 2. Registrar tu misma clase DatabaseRepository para inyectarla en las pantallas Web


// 3. Habilitar Razor Components interactivos (Blazor Server en .NET 8)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();