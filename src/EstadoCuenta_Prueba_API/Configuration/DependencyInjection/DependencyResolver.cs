using Microsoft.Extensions.DependencyInjection;
using Infrastructure.DataBase.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Configuration.AutoMapper;


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
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            return services;
        }
    }
}
