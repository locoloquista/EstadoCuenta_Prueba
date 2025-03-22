using BussinesLogic;
using Configuration.DependencyInjection;
using Infrastructure.DataBase.DataAccess;
using Infrastructure.DataBase.DBContext;
using Infrastructure.Mapping;
using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.Mapping;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Servicios declarados en Configuration.DI.DependencyResolver
var configuration = builder.Configuration;
builder.Services.ConfigureInfraestructure(configuration);
builder.Services.ConfigureBussinesLogic();


// Configurar la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Registrar ApplicationDbContext con la conexión de base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
// Registrar SqlConnection
builder.Services.AddTransient<SqlConnection>(provider => new SqlConnection(connectionString));


builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IParser, Parser>();
builder.Services.AddTransient<IClienteBOL, ClienteBOL>();
builder.Services.AddTransient<IClienteDAO, ClienteDAO>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
