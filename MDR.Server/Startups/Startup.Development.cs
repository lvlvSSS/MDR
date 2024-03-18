using Autofac;
using Autofac.Extensions.DependencyInjection;
using MDR.Data.Model.Jwt;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Caching.Memory;
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
            // swagger only used in development.
            services.AddSwaggerGen();
            // Add services to the container，并将 Controller 交给 autofac 容器来处理.
            services.AddControllers().AddControllersAsServices();

            services.AddEndpointsApiExplorer();
            // jwt options
            services.AddOptions<JwtTokenParameterOptions>()
                .Bind(configuration.GetSection(JwtTokenParameterOptions.Name))
                .ValidateDataAnnotations();
            // configure memory cache. default is local memory cache.
            services.AddDistributedMemoryCache();
            services.Configure<MemoryDistributedCacheOptions>(configuration.GetSection(nameof(MemoryCacheOptions)));

            // http logging configuration.
            services.AddHttpLogging(builder =>
            {
                builder.LoggingFields = HttpLoggingFields.All;
                /*
                builder.RequestHeaders.Add("My-Request-Header");
                builder.ResponseHeaders.Add("My-Response-Header");
                */
                builder.MediaTypeOptions.AddText("application/javascript");
                builder.RequestBodyLogLimit = 500 * 1024;
                builder.ResponseBodyLogLimit = 500 * 1024;
            });
            // configure NLog as http logging.
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddNLog("NLog.config");

                bool isWindows =
#if NETCOREAPP
                    OperatingSystem.IsWindows();
#elif NETFRAMEWORK
                    Environment.OSVersion.Platform == PlatformID.Win32NT;
#else
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
                if (isWindows)
                {
                    builder.AddEventLog(settings =>
                    {
                        settings.LogName = "MDR";
                        settings.SourceName = "MDR";
                        settings.Filter = (_, level) => level >= LogLevel.Error;
                    });
                }
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
            // swagger only used in development.
            if (webHostEnvironment.IsDevelopment())
            {
                // 异常处理
                app.UseDeveloperExceptionPage();
                // swagger
                app.UseSwagger();
                app.UseSwaggerUI();
                // 静态文件浏览, 默认指向 wwwroot
                app.UseFileServer(new FileServerOptions() { EnableDirectoryBrowsing = true });
            }

            // 通过此方法获取 autofac 的 DI容器
            _autofacContainer = app.ApplicationServices.GetAutofacRoot();

            // Configure the HTTP request pipeline.
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            // use http logging
            app.UseHttpLogging();
            // needed for HTTP response body with an API Controller.
            //app.UseMiddleware<NLogResponseBodyMiddleware>(new NLogResponseBodyMiddlewareOptions());

            // cors
            app.UseCors();
            // 终结点
            app.UseEndpoints(
                endpoints => { endpoints.MapControllers(); }
            );
        }
    }
}