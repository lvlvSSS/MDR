using Microsoft.AspNetCore.HttpLogging;
using NLog.Extensions.Logging;

namespace MDR.Server.Startup
{
    public class StartupDevelopment(
       IConfiguration configuration,
       IWebHostEnvironment webHostEnvironment)
    {
        public IConfiguration Configuration { get; } = configuration;

        public IWebHostEnvironment WebHostEnvironment { get; set; } = webHostEnvironment;

        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine($"{nameof(ConfigureServices)}: {WebHostEnvironment.EnvironmentName}");
            // Add services to the container.
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpLogging(builder =>
            {
                builder.LoggingFields = HttpLoggingFields.All;
                builder.RequestHeaders.Add("My-Request-Header");
                builder.ResponseHeaders.Add("My-Response-Header");
                builder.MediaTypeOptions.AddText("application/javascript");
                builder.RequestBodyLogLimit = 4096;
                builder.ResponseBodyLogLimit = 4096;
            });
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddNLog("NLog.config");
            });

        }

        public void Configure(IApplicationBuilder app)
        {
            Console.WriteLine($"{nameof(Configure)}: {WebHostEnvironment.EnvironmentName}");
            // Configure the HTTP request pipeline.
            if (WebHostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpLogging();
            // needed for HTTP response body with an API Controller.
            //app.UseMiddleware<NLogResponseBodyMiddleware>(new NLogResponseBodyMiddlewareOptions());

            app.UseCors();
            app.UseEndpoints(
                    endpoints => { endpoints.MapControllers(); }
            );
            
        }
    }
}