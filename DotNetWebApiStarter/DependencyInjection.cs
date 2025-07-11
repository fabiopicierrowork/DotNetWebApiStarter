using DotNetWebApiStarter.Data.Repositories.Interfaces;
using DotNetWebApiStarter.Data.Repositories;
using DotNetWebApiStarter.Services.Interfaces;
using DotNetWebApiStarter.Services;
using DotNetWebApiStarter.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DotNetWebApiStarter
{
    public static class DependencyInjection
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
        }

        public static void ConfigureAutoMappers(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AuthProfile));
            services.AddAutoMapper(typeof(UserProfile));
            services.AddAutoMapper(typeof(ProductProfile));
        }

        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            });
        }
    }
}
