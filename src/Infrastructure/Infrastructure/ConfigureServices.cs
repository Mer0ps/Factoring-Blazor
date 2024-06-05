using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;
public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("FactoringConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<FactoringContext>(opt =>
        opt.UseNpgsql(connectionString));

        services.AddScoped<IFactoringContext>(provider => provider.GetRequiredService<FactoringContext>());

        services.AddScoped<FactoringContextInitialiser>();

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}
