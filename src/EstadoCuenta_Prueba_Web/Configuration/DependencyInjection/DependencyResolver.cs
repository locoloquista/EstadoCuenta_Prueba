using BussinesLogic;
using Configuration.AutoMapper;
using Infraestructure.Mapping;
using Infraestructure.PdfGeneration;
using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.ConsumerServices;
using InterfaceAdapter.Mapping;
using InterfaceAdapter.PdfGeneration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Consumer;
using Services.Consumer.ConsumerFactory;

namespace Configuration.DependencyInjection
{
    public static class DependencyResolver
    {
        public static void ConfigureInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            //Configuracion de AutoMapper
            services.AddAndConfigMapper();

            // Registro de la configuración de ApiSettings
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

            //Inyecion de dependencias
            services.AddScoped<IParser, Parser>();

        }

        public static void ConfigureBussinesLogic(this IServiceCollection services)
        {
            //Inyecion de dependencias
            services.AddScoped<IClienteBOL, ClienteBOL>();
            services.AddScoped<ITarjetaCreditoBOL, TarjetaCreditoBOL>();
            services.AddScoped<IEstadoCuentaBOL, EstadoCuentaBOL>();
            services.AddScoped<ITransaccionesBOL, TransaccionesBOL>();

        }

        public static void ConfigureConsumerServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Inyecion de dependencias
            services.AddScoped<IClienteServices, ClienteServices>();
            services.AddScoped<ITarjetaCreditoServices, TarjetaCreditoServices>();
            services.AddScoped<IEstadoCuentaServices, EstadoCuentaServices>();
            services.AddScoped<ITransaccionesServices, TransaccionesServices>();
            services.AddScoped<IConsumerFactory, ConsumerFactory>();
            services.AddScoped<IRestApiConsumer>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var stringValue = configuration.GetValue<string>("ApiSettings:EstadoCuentaApi_url");
                return new RestApiConsumer(stringValue);
            });
            services.AddTransient<IPdfGeneratorServices, PdfGeneratorServices>();
            services.AddHttpContextAccessor();
        }
    }
}
