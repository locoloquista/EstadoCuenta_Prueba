using BussinesLogic;
using Configuration.AutoMapper;
using Infrastructure.DataBase.DataAccess;
using Infrastructure.DataBase.DBContext;
using Infrastructure.Mapping;
using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Configuration.DependencyInjection
{
    public static class DependencyResolver
    {
        public static IServiceCollection ConfigureInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            //Inyectar contexto de base de datos
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );


            //Configuracion de AutoMapper
            services.AddAndConfigMapper();


            //Inyecion de dependencias
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IParser, Parser>();


            return services;
        }

        public static IServiceCollection ConfigureBussinesLogic(this IServiceCollection services)
        {
            //Inyecion de dependencias
            services.AddTransient<IClienteBOL, ClienteBOL>();

            return services;
        }

        public static IServiceCollection ConfigureDataAccess(this IServiceCollection services)
        {
            //Inyecion de dependencias
            services.AddTransient<IClienteDAO, ClienteDAO>();

            return services;
        }
    }
}
