using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MDR.Data.Model.Jwt;
using MDR.Server.Samples.Middlewares;
using MDR.Server.Samples.Models;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using NLog.Extensions.Logging;
using NLog.Filters;

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
            // Add services to the container，并将 Controller 交给 autofac 容器来处理.
            services.AddControllers().AddControllersAsServices().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
            // add third-party [fluent-validation] as validation.
            /*
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssemblyContaining<CreateUserNameDtoValidator>();
            */

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
            
            // 通过 类似于 ThreadLocal<T>， C#中使用 AsyncLocal<T> 实现 HttpContext 注入到每个执行线程中去。
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
            if (!webHostEnvironment.IsDevelopment())
            {
                // 异常处理
                app.UseExceptionHandler(errorApp => errorApp.UseMiddleware<MdrJsonExceptionMiddleware>());
            }

            // 通过此方法获取 autofac 的 DI容器
            _autofacContainer = app.ApplicationServices.GetAutofacRoot();
            // Configure the HTTP request pipeline.
            app.UseRouting();
            app.UseSession();
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