using BussinesLogic;
using Configuration.AutoMapper;
using Infraestructure.ConsumerServices;
using Infraestructure.Mapping;
using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.ConsumerServices;
using InterfaceAdapter.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Configuration.DependencyInjection
{
    public static class DependencyResolver
    {
        public static void ConfigureInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            
            //Configuracion de AutoMapper
            services.AddAndConfigMapper();


            //Inyecion de dependencias
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

        public static void ConfigureConsumerServices(this IServiceCollection services)
        {
            //Inyecion de dependencias
            services.AddTransient<IClienteServices, ClienteServices>();
            services.AddTransient<ITarjetaCreditoServices, TarjetaCreditoServices>();
            services.AddTransient<IEstadoCuentaServices, EstadoCuentaServices>();
            services.AddTransient<ITransaccionesServices, TransaccionesServices>();
        }
    }
}
