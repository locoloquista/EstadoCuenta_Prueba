using Microsoft.Extensions.DependencyInjection;

namespace Configuration.AutoMapper
{
    public static class MappingExtensions
    {
        public static IServiceCollection AddAndConfigMapper(this IServiceCollection services)
        {
            return services.AddAutoMapper(typeof(MappingProfile));
        }
    }
}
