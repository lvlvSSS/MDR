using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpLogging;
using NLog.Extensions.Logging;

namespace MDR.Server.Startups
{
    public class StartupDevelopment(
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment)
    {
        private ILifetimeScope? _autofacContainer;

        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine($"{nameof(ConfigureServices)}: {webHostEnvironment.EnvironmentName}");
            // Add services to the container.
            services.AddControllers()
                .AddControllersAsServices(); // 将 Controller 交给 autofac 容器来处理.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpLogging(builder =>
            {
                builder.LoggingFields = HttpLoggingFields.All;
                builder.RequestHeaders.Add("My-Request-Header");
                builder.ResponseHeaders.Add("My-Response-Header");
                builder.MediaTypeOptions.AddText("application/javascript");
                builder.RequestBodyLogLimit = 64 * 1024;
                builder.ResponseBodyLogLimit = 64 * 1024;
            });
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddNLog("NLog.config");
            });
        }

        // 1. ConfigureContainer 用于使用 Autofac 进行服务注册
        // 2. 该方法在 ConfigureServices 之后运行，所以这里的注册会覆盖之前的注册
        // 3. 不要 build 容器，不要调用 builder.Populate()，AutofacServiceProviderFactory 已经做了这些工作了
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // 将服务注册划分为模块，进行注册
            builder.RegisterModule(new AutofacModule());
        }

        public void Configure(IApplicationBuilder app)
        {
            Console.WriteLine($"{nameof(Configure)}: {webHostEnvironment.EnvironmentName}");
            Console.WriteLine($"nelson: {configuration["nelson"]}");
            // 通过此方法获取 autofac 的 DI容器
            _autofacContainer = app.ApplicationServices.GetAutofacRoot();
            // Configure the HTTP request pipeline.
            if (webHostEnvironment.IsDevelopment())
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