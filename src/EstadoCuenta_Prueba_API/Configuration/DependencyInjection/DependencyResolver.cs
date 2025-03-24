using BussinesLogic;
using Configuration.AutoMapper;
using Infrastructure.DataBase.DataAccess;
using Infrastructure.DataBase.DBContext;
using Infrastructure.Mapping;
using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.Mapping;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Configuration.DependencyInjection
{
    public static class DependencyResolver
    {
        public static void ConfigureInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            //Inyectar contexto de base de datos
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            // Registrar ApplicationDbContext con la conexión de base de datos
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            // Registrar SqlConnection
            services.AddTransient<SqlConnection>(provider => new SqlConnection(connectionString));


            //Configuracion de AutoMapper
            services.AddAndConfigMapper();


            //Inyecion de dependencias
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IParser, Parser>();

        }

        public static void ConfigureBussinesLogic(this IServiceCollection services)
        {
            //Inyecion de dependencias
            services.AddTransient<IClienteBOL, ClienteBOL>();
            services.AddTransient<ITarjetaCreditoBOL, TarjetaCreditoBOL>();
            services.AddTransient<IEstadoCuentaBOL, EstadoCuentaBOL>();
            services.AddTransient<ITransaccionesBOL, TransaccionesBOL>();

        }

        public static void ConfigureDataAccess(this IServiceCollection services)
        {
            //Inyecion de dependencias
            services.AddTransient<IClienteDAO, ClienteDAO>();
            services.AddTransient<ITarjetaCreditoDAO, TarjetaCreditoDAO>();
            services.AddTransient<IEstadoCuentaDAO, EstadoCuentaDAO>();
            services.AddTransient<ITransaccionesDAO, TransaccionesDAO>();
        }
    }
}
