using Autofac;
using Autofac.Extensions.DependencyInjection;
using MDR.Data.Model.Jwt;
using Microsoft.AspNetCore.HttpLogging;
using NLog.Extensions.Logging;

#nullable enable
namespace MDR.Server.Startups
{
    /// <summary>
    /// Startup 启动时，因为 <see cref="T:Microsoft.Extensions.DependencyInjection.DefaultServiceProviderFactory" /> 或者 <see cref="T:Autofac.Extensions.DependencyInjection.AutofacServiceProviderFactory" /> 还未创建，
    /// 因此是用 <see cref="T:Microsoft.AspNetCore.Hosting.GenericWebHostBuilder.HostServiceProvider"/> 作为 IServiceProvider的实现类。
    /// 而 HostServiceProvider 实现类的 GetService 只提供 <see cref="T:Microsoft.Extensions.Hosting.IHostingEnvironment" />，
    /// <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostEnvironment" />，
    /// <see cref="T:Microsoft.Extensions.Hosting.IHostEnvironment" />，
    /// <see cref="T:Microsoft.Extensions.Configuration.IConfiguration" /> 的注入。
    /// 所以，Startup 构造函数的参数只能使用以上几种类型。
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="webHostEnvironment"></param>
    public class Startup(
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
            services.Configure<JwtTokenParameterOptions>(configuration.GetSection("Jwt:Token"));
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

            // 通过此方法获取 autofac 的 DI容器
            _autofacContainer = app.ApplicationServices.GetAutofacRoot();

            // Configure the HTTP request pipeline.
            /*
            if (webHostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            */

            app.UseRouting(); // attention, UseRouting must be used bofore UseEndpoints.
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