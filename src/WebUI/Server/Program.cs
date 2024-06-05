using Application;
using Domain.Constants;
using Hangfire;
using Hangfire.MemoryStorage;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Mx.Blazor.DApp.Components;
using Mx.Blazor.DApp.Server.BackgroundServices;
using Mx.Blazor.DApp.Server.Services;
using Mx.Blazor.DApp.Server.Services.Interfaces;
using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Provider;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddOpenApiDocument();
builder.Services.AddRazorPages().AddNewtonsoftJson();
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();

builder.Services.AddSignalR();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);


builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

//builder.Services.AddHangfire(x => x.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("FactoringConnection")), new PostgreSqlStorageOptions
//{
//    SchemaName = "hangfire",
//}));

builder.Services.AddHangfire(c => c.UseMemoryStorage());

builder.Services.Configure<IPFSSettings>(builder.Configuration.GetSection("IPFS"));
builder.Services.Configure<EventNotifierSettings>(builder.Configuration.GetSection("EventNotifier"));

builder.Services.AddHangfireServer();
builder.Services.AddHostedService<RabbitMQService>();

builder.Services.AddSingleton(new ApiNetworkConfiguration(ContractConstants.NETWORK));
builder.Services.AddSingleton<ApiProvider>();

builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.InNamespaces(typeof(BotService).Namespace))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:SecurityKey"));
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<FactoringContextInitialiser>();
        await initialiser.InitialiseAsync();
    }
    catch (Exception ex)
    {
        var loggerDb = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        loggerDb.LogError(ex, "An error occurred during database initialisation.");

        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseOpenApi();
    app.UseSwaggerUi();
}
else
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorPages();
app.MapControllers();

app.MapHub<NotificationHub>("/notificationHub");

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Mx.Blazor.DApp.Client._Imports).Assembly);

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<IBotService>(nameof(IBotService.Execute), x => x.Execute(), "*/10 * * * * *");
RecurringJob.AddOrUpdate<IBotService>(nameof(IBotService.GetEuriborRate), x => x.GetEuriborRate(), "0 2 * * *");
RecurringJob.AddOrUpdate<IBotService>(nameof(IBotService.CalculateScore), x => x.CalculateScore(), "0 2 * * *");

app.Run();
