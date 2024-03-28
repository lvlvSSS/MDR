using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityModel;
using MDR.Data.Model.Jwt;
using MDR.Server.Samples.Middlewares;
using MDR.Server.Samples.Models;
using MDR.Server.Samples.PostConfigures;
using MDR.Server.Startups;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MDR.WebApi", Version = "v1" });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };
                c.AddSecurityRequirement(securityRequirement);
            });
            // Add services to the container，并将 Controller 交给 autofac 容器来处理.
            services.AddControllers().AddControllersAsServices().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            });

            var jwtOptions = configuration.GetSection(JwtTokenParameterOptions.Name).Get<JwtTokenParameterOptions>();

            // add jwt bearer auth
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = jwtOptions!.DefaultTokenValidationParameters;
                    //options.EventsType = typeof(AppJwtBearerEvents);
                });


            // add api behavior post configuration in model-binding.
            services.AddSingleton<IPostConfigureOptions<ApiBehaviorOptions>, ApiBehaviorOptionPostSetup>();

            // 添加 ActionFilters
            //services.AddScoped<MdrExceptionFilter>();
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

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
            // swagger only used in development.
            if (webHostEnvironment.IsDevelopment())
            {
                // 异常处理
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/weatherforecast/error");
                //app.UseExceptionHandler(errorApp => errorApp.UseMiddleware<MdrJsonExceptionMiddleware>());
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