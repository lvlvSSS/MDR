using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDR.Server.Samples.Middlewares;

/// <summary>
/// 基于约定的 Middleware:
/// 1. 拥有公共（public）构造函数，且该构造函数至少包含一个类型为RequestDelegate的参数
/// 2. 拥有名为Invoke或InvokeAsync的公共（public）方法，必须包含一个类型为HttpContext的方法参数，且该参数必须位于第一个参数的位置，另外该方法必须返回Task类型
/// 3. 构造函数中的其他参数可以通过依赖注入（DI）填充，也可以通过UseMiddleware传参进行填充。
///     3.1 通过DI填充时，只能接收 Transient 和 Singleton 的DI参数。这是由于中间件是在应用启动时构造的（而不是按请求构造），所以当出现 Scoped 参数时，构造函数内的DI参数生命周期与其他不共享，如果想要共享，则必须将Scoped DI参数添加到Invoke/InvokeAsync来进行使用。
///     3.2 通过UseMiddleware传参时，构造函数内的DI参数和非DI参数顺序没有要求，传入UseMiddleware内的参数顺序也没有要求，建议将非DI参数放到前面，DI参数放到后面
/// 4. Invoke/InvokeAsync的其他参数也能够通过依赖注入（DI）填充，可以接收 Transient、Scoped 和 Singleton 的DI参数
///
/// 基于工厂的强类型 Middleware, 要求实现接口 <see cref="T:Microsoft.AspNetCore.Http.IMiddleware" />.
/// 注意：
///     1. 该 IMiddleware 默认是由 <see cref="T:Microsoft.AspNetCore.Http.IMiddlewareFactory" /> 的默认实现类 <see cref="T:Microsoft.AspNetCore.Http.MiddlewareFactory" /> 来进行创建(可以通过 <see cref="Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.Replace" /> 来替换默认的 MiddlewareFactory)
///     2. 基于约定的中间件实例都是 Singleton；而基于工厂的中间件实例可以是 Singleton、Scoped 和 Transient（当然，不建议注册为 Singleton）
///     3. 所有IMiddleware的Middleware不能只是IApplicationBuilder来进行Use，还需要 <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 进行注册，这是因为默认实现的MiddlewareFactory 会从容器中获取已经注册的类型，如果没有注册就获取不到。
///     4. IMiddleware 无法通过UseMiddleware向中间件的构造函数传参
///     5. 基于约定的中间件实例可以在Invoke/InvokeAsync中添加更多的依赖注入参数；而基于工厂的中间件只能按照IMiddleware的接口定义进行实现
/// 
/// </summary>
/// SampleMiddleware 为基于约定的中间件
public class SampleMiddleware(
    RequestDelegate next,
    IConfiguration configuration
    )
{
    private readonly RequestDelegate _next = next;
    private readonly IConfiguration _configuration = configuration;

    public async Task InvokeAsync(
        HttpContext context,
        IWebHostEnvironment environment
    )
    {
        Console.WriteLine($"SampleMiddleware start,{environment.EnvironmentName} run before next middleware ...");
        await _next(context);
        Console.WriteLine($"SampleMiddleware end,{environment.EnvironmentName} run after next middleware");
    }
}


public static class SampleMiddlewareExtensions
{
    public static IApplicationBuilder UseSampleMiddleware(this IApplicationBuilder app)
    => app.UseMiddleware<SampleMiddleware>();
}