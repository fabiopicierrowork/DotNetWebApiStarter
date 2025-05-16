using DotNetWebApiStarter.Data.Repositories.Interfaces;
using DotNetWebApiStarter.Data.Repositories;
using DotNetWebApiStarter.Services.Interfaces;
using DotNetWebApiStarter.Services;
using DotNetWebApiStarter.Mappings;

namespace DotNetWebApiStarter
{
    public static class DependencyInjection
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
        }

        public static void ConfigureAutoMappers(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ProductProfile));
        }
    }
}
