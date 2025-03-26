using Configuration.DependencyInjection;
using Rotativa.AspNetCore;
using InterfaceAdapter.PdfGeneration;
using Infraestructure.PdfGeneration;

var builder = WebApplication.CreateBuilder(args);

// Configura Rotativa
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Agregar servicios de sesión y caché distribuida
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//Servicios declarados en Configuration.DI.DependencyResolver
var configuration = builder.Configuration;
builder.Services.ConfigureInfraestructure(configuration);
builder.Services.ConfigureBussinesLogic();
builder.Services.ConfigureConsumerServices(configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cliente}/{action=ListadoClientes}/{id?}");

// Configura Rotativa
RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

app.Run();

