using DotNetWebApiStarter;
using DotNetWebApiStarter.Middlewares;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureDatabaseContext();
        builder.Services.ConfigureRepositories();
        builder.Services.ConfigureServices();
        builder.Services.ConfigureAutoMappers();

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseMiddleware<DatabaseExceptionHandlerMiddleware>();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}