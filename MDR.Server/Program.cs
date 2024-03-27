using Autofac.Extensions.DependencyInjection;
using MDR.Data.Model.Jwt;
using MDR.Infrastructure.Extensions;
using MDR.Server.Startups;
using Microsoft.AspNetCore.HttpLogging;
using NLog.Extensions.Logging;

namespace MDR.Server;

public class Program
{
    public static void Main(string[] args)
    {
        HostBuilderWithWebHost(args);
    }

    private static void HostBuilderWithWebHost(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                config
                    .AddJsonFile("jwt.json", optional: false, reloadOnChange: false)                        // jwt config
                    .AddJsonFile($"jwt.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)   // jwt-environment config
                    
                    .AddJsonFile($"psql.json", optional: true, reloadOnChange: true)                        // postgresql config
                    .AddJsonFile($"psql.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)  // postgresql-environment config
                    .AddEnvironmentVariables("PSQL_");                                                          // add postgresql environment
                //.AddCommandLine(args);
            })
            /*
            .ConfigureServices((context, _) =>
            {
                // 注册Configuration的变更监听
                var targetConfig = context.Configuration;
                context.Configuration.GetReloadToken()
                    .RegisterChangeCallback(
                        (o => { ConfigurationChanged(targetConfig); }),
                        null);
            })
            */
            .ConfigureWebHostDefaults(webBuilder => { _ = webBuilder.UseStartup(typeof(Startup).Assembly.FullName!); })
            .Build().Run();
    }

    private static void ConfigurationChanged(IConfiguration configuration)
    {
        // 每次发生变更之后，CancellationTokenSource会重新new一个，因此需要重新注册。
        configuration.GetReloadToken().RegisterChangeCallback(
            o => { ConfigurationChanged(configuration); }, null);
    }

    #region web application 启动方式

    private static void WebApplicationHost(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpLogging(builder =>
        {
            builder.LoggingFields = HttpLoggingFields.All;
            builder.RequestHeaders.Add("My-Request-Header");
            builder.ResponseHeaders.Add("My-Response-Header");
            builder.MediaTypeOptions.AddText("application/javascript");
            builder.RequestBodyLogLimit = 4096;
            builder.ResponseBodyLogLimit = 4096;
        });
        builder.Services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddNLog("NLog.config");
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseHttpLogging();
        // needed for HTTP response body with an API Controller.
        //app.UseMiddleware<NLogResponseBodyMiddleware>(new NLogResponseBodyMiddlewareOptions());
        app.MapControllers();


        app.Run();
    }

    #endregion
}